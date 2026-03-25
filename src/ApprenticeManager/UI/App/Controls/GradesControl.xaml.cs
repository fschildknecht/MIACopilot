using ApprenticeManager.Models;
using ApprenticeManager.Services;
using ApprenticeManager.UI.App.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace ApprenticeManager.UI.App.Controls;

public partial class GradesControl : UserControl
{
    private IServiceProvider? _serviceProvider;

    public GradesControl()
    {
        InitializeComponent();
    }

    public void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Loaded += async (_, _) => await LoadApprenticesAsync();
    }

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

    private async void ApprenticeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ApprenticeComboBox.SelectedItem is not Apprentice apprentice)
        {
            SubjectsGrid.ItemsSource = null;
            GradesGrid.ItemsSource = null;
            return;
        }
        await LoadSubjectsAsync(apprentice.Id);
    }

    private async Task LoadSubjectsAsync(int apprenticeId)
    {
        using var scope = _serviceProvider!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGradeService>();
        var subjects = await service.GetSubjectsByApprenticeAsync(apprenticeId);
        SubjectsGrid.ItemsSource = subjects.ToList();
        GradesGrid.ItemsSource = null;
        GradesHeader.Text = "Noten";
    }

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

    private async Task LoadGradesAsync(int subjectId)
    {
        using var scope = _serviceProvider!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGradeService>();
        var grades = await service.GetGradesBySubjectAsync(subjectId);
        GradesGrid.ItemsSource = grades.ToList();
    }

    private async void AddSubjectButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApprenticeComboBox.SelectedItem is not Apprentice apprentice)
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
                await service.AddSubjectAsync(new Subject { Name = dialog.SubjectName, ApprenticeId = apprentice.Id });
                await LoadSubjectsAsync(apprentice.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void DeleteSubjectButton_Click(object sender, RoutedEventArgs e)
    {
        if (SubjectsGrid.SelectedItem is not Subject subject) return;
        if (ApprenticeComboBox.SelectedItem is not Apprentice apprentice) return;

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
                await LoadSubjectsAsync(apprentice.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

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
                if (ApprenticeComboBox.SelectedItem is Apprentice apprentice)
                    await LoadSubjectsAsync(apprentice.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

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
                if (ApprenticeComboBox.SelectedItem is Apprentice apprentice)
                    await LoadSubjectsAsync(apprentice.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
