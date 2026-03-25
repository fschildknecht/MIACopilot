using ApprenticeManager.Models;
using ApprenticeManager.Services;
using ApprenticeManager.UI.App.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace ApprenticeManager.UI.App.Controls;

public partial class LearningJournalControl : UserControl
{
    private IServiceProvider? _serviceProvider;

    public LearningJournalControl()
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
            JournalGrid.ItemsSource = null;
            return;
        }
        await LoadJournalAsync(apprentice.Id);
    }

    private async Task LoadJournalAsync(int apprenticeId)
    {
        using var scope = _serviceProvider!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILearningJournalService>();
        var entries = await service.GetByApprenticeAsync(apprenticeId);
        JournalGrid.ItemsSource = entries.ToList();
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApprenticeComboBox.SelectedItem is not Apprentice apprentice)
        {
            MessageBox.Show("Bitte zuerst einen Lernenden auswählen.", "Hinweis",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialog = new JournalDialog { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                dialog.Result.ApprenticeId = apprentice.Id;
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ILearningJournalService>();
                await service.AddAsync(dialog.Result);
                await LoadJournalAsync(apprentice.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (JournalGrid.SelectedItem is not LearningJournal selected) return;
        if (ApprenticeComboBox.SelectedItem is not Apprentice apprentice) return;

        var dialog = new JournalDialog(selected) { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ILearningJournalService>();
                await service.UpdateAsync(dialog.Result);
                await LoadJournalAsync(apprentice.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (JournalGrid.SelectedItem is not LearningJournal selected) return;
        if (ApprenticeComboBox.SelectedItem is not Apprentice apprentice) return;

        var confirm = MessageBox.Show(
            $"Journaleintrag '{selected.Titel}' wirklich löschen?",
            "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (confirm == MessageBoxResult.Yes)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ILearningJournalService>();
                await service.DeleteAsync(selected.Id);
                await LoadJournalAsync(apprentice.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApprenticeComboBox.SelectedItem is Apprentice apprentice)
            await LoadJournalAsync(apprentice.Id);
    }
}
