using ApprenticeManager.Models;
using ApprenticeManager.Services;
using ApprenticeManager.UI.App.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace ApprenticeManager.UI.App.Controls;

public partial class ApprenticesControl : UserControl
{
    private IServiceProvider? _serviceProvider;

    public ApprenticesControl()
    {
        InitializeComponent();
    }

    public void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Loaded += async (_, _) => await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        using var scope = _serviceProvider!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IApprenticeService>();
        var apprentices = await service.GetAllAsync();
        ApprenticesGrid.ItemsSource = apprentices.ToList();
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ApprenticeDialog { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IApprenticeService>();
                await service.AddAsync(dialog.Result);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApprenticesGrid.SelectedItem is not Apprentice selected) return;

        var dialog = new ApprenticeDialog(selected) { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IApprenticeService>();
                await service.UpdateAsync(dialog.Result);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (ApprenticesGrid.SelectedItem is not Apprentice selected) return;

        var confirm = MessageBox.Show(
            $"Lernenden '{selected.FullName}' wirklich löschen?\nAlle Noten und Journaleinträge werden ebenfalls gelöscht.",
            "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (confirm == MessageBoxResult.Yes)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IApprenticeService>();
                await service.DeleteAsync(selected.Id);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadDataAsync();
    }
}
