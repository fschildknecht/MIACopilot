using ApprenticeManager.Models;
using ApprenticeManager.Services;
using ApprenticeManager.UI.App.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace ApprenticeManager.UI.App.Controls;

/// <summary>UserControl showing a split view of subjects and grades for an apprentice.</summary>
public partial class GradesControl : UserControl
{
    private IServiceProvider? _serviceProvider;
    private int? _activeApprenticeId;

    public GradesControl()
    {
        InitializeComponent();
    }

    /// <summary>Injects the DI container and loads the apprentice list for the selector.</summary>
    public void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Loaded += async (_, _) => await LoadApprenticesAsync();
    }

    /// <summary>
    /// Locks the control to a specific apprentice (used in Lernender-mode).
    /// Hides the apprentice selector and loads subjects immediately.
    /// </summary>
    public async Task SetApprentice(int apprenticeId)
    {
        _activeApprenticeId = apprenticeId;
        ApprenticeSelector.Visibility = Visibility.Collapsed;
        await LoadSubjectsAsync(apprenticeId);
    }

    /// <summary>Restores the trainer-mode view with the apprentice selector visible.</summary>
    public async Task SetTrainerMode()
    {
        _activeApprenticeId = null;
        ApprenticeSelector.Visibility = Visibility.Visible;
        SubjectsGrid.ItemsSource = null;
        GradesGrid.ItemsSource = null;
        GradesHeader.Text = "Noten";
        await LoadApprenticesAsync();
    }

    /// <summary>Refreshes the apprentice combo box while preserving the current selection.</summary>
    private async Task LoadApprenticesAsync()
    {
        using var scope = _serviceProvider!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IApprenticeService>();
        var apprentices = await service.GetAllAsync();
        var current = ApprenticeComboBox.SelectedItem as Apprentice;
        ApprenticeComboBox.ItemsSource = apprentices.ToList();
        if (current != null)
            ApprenticeComboBox.SelectedItem = ApprenticeComboBox.Items.Cast<Apprentice>()
                .FirstOrDefault(a => a.Id == current.Id);
    }

    /// <summary>Handles apprentice selection changes in trainer mode.</summary>
    private async void ApprenticeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ApprenticeComboBox.SelectedItem is not Apprentice apprentice)
        {
            SubjectsGrid.ItemsSource = null;
            GradesGrid.ItemsSource = null;
            return;
        }
        _activeApprenticeId = apprentice.Id;
        await LoadSubjectsAsync(apprentice.Id);
    }

    /// <summary>Loads all subjects for the given apprentice and resets the grades panel.</summary>
    private async Task LoadSubjectsAsync(int apprenticeId)
    {
        using var scope = _serviceProvider!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGradeService>();
        var subjects = await service.GetSubjectsByApprenticeAsync(apprenticeId);
        SubjectsGrid.ItemsSource = subjects.ToList();
        GradesGrid.ItemsSource = null;
        GradesHeader.Text = "Noten";
    }

    /// <summary>Loads the grades for the selected subject and updates the header.</summary>
    private async void SubjectsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SubjectsGrid.SelectedItem is not Subject subject)
        {
            GradesGrid.ItemsSource = null;
            return;
        }
        GradesHeader.Text = $"Noten: {subject.Name}";
        await LoadGradesAsync(subject.Id);
    }

    /// <summary>Loads all grades for the given subject.</summary>
    private async Task LoadGradesAsync(int subjectId)
    {
        using var scope = _serviceProvider!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGradeService>();
        var grades = await service.GetGradesBySubjectAsync(subjectId);
        GradesGrid.ItemsSource = grades.ToList();
    }

    /// <summary>Opens the add-subject dialog and persists the new subject if confirmed.</summary>
    private async void AddSubjectButton_Click(object sender, RoutedEventArgs e)
    {
        if (_activeApprenticeId == null)
        {
            MessageBox.Show("Bitte zuerst einen Lernenden auswählen.", "Hinweis",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialog = new SubjectDialog { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IGradeService>();
                await service.AddSubjectAsync(new Subject { Name = dialog.SubjectName, ApprenticeId = _activeApprenticeId.Value });
                await LoadSubjectsAsync(_activeApprenticeId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Prompts for confirmation and deletes the selected subject along with all its grades.</summary>
    private async void DeleteSubjectButton_Click(object sender, RoutedEventArgs e)
    {
        if (SubjectsGrid.SelectedItem is not Subject subject) return;
        if (_activeApprenticeId == null) return;

        var confirm = MessageBox.Show(
            $"Fach '{subject.Name}' und alle dazugehörigen Noten wirklich löschen?",
            "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (confirm == MessageBoxResult.Yes)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IGradeService>();
                await service.DeleteSubjectAsync(subject.Id);
                await LoadSubjectsAsync(_activeApprenticeId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Opens the add-grade dialog and persists the new grade if confirmed.</summary>
    private async void AddGradeButton_Click(object sender, RoutedEventArgs e)
    {
        if (SubjectsGrid.SelectedItem is not Subject subject)
        {
            MessageBox.Show("Bitte zuerst ein Fach auswählen.", "Hinweis",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialog = new GradeDialog { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IGradeService>();
                await service.AddGradeAsync(new Grade
                {
                    SubjectId = subject.Id,
                    Value = dialog.GradeValue,
                    Date = dialog.GradeDate,
                    Notes = dialog.GradeNotes
                });
                await LoadGradesAsync(subject.Id);
                if (_activeApprenticeId.HasValue)
                    await LoadSubjectsAsync(_activeApprenticeId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Prompts for confirmation and deletes the selected grade.</summary>
    private async void DeleteGradeButton_Click(object sender, RoutedEventArgs e)
    {
        if (GradesGrid.SelectedItem is not Grade grade) return;
        if (SubjectsGrid.SelectedItem is not Subject subject) return;

        var confirm = MessageBox.Show(
            $"Note {grade.Value:F1} vom {grade.Date:dd.MM.yyyy} wirklich löschen?",
            "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (confirm == MessageBoxResult.Yes)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IGradeService>();
                await service.DeleteGradeAsync(grade.Id);
                await LoadGradesAsync(subject.Id);
                if (_activeApprenticeId.HasValue)
                    await LoadSubjectsAsync(_activeApprenticeId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

