using ApprenticeManager.Models;
using ApprenticeManager.Services;
using ApprenticeManager.UI.App.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace ApprenticeManager.UI.App.Controls;

/// <summary>UserControl that lists all apprentices with search and provides CRUD actions.</summary>
public partial class ApprenticesControl : UserControl
{
    private IServiceProvider? _serviceProvider;

    /// <summary>Raised after an apprentice is added or deleted so the role switch can refresh.</summary>
    public event EventHandler? ApprenticeListChanged;

    public ApprenticesControl()
    {
        InitializeComponent();
    }

    /// <summary>Injects the DI container and triggers the first data load once the control is ready.</summary>
    public void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Loaded += async (_, _) => await LoadDataAsync();
    }

    /// <summary>Loads all apprentices (or a filtered subset) and binds them to the grid.</summary>
    private async Task LoadDataAsync(string? searchTerm = null)
    {
        using var scope = _serviceProvider!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IApprenticeService>();
        var apprentices = string.IsNullOrWhiteSpace(searchTerm)
            ? await service.GetAllAsync()
            : await service.SearchAsync(searchTerm);
        ApprenticesGrid.ItemsSource = apprentices.ToList();
    }

    /// <summary>Filters the grid as the user types in the search box.</summary>
    private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        await LoadDataAsync(SearchBox.Text);
    }

    /// <summary>Clears the search box and reloads all apprentices.</summary>
    private async void ClearSearch_Click(object sender, RoutedEventArgs e)
    {
        SearchBox.Text = string.Empty;
        await LoadDataAsync();
    }

    /// <summary>Opens the add dialog and persists the new apprentice if confirmed.</summary>
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
                await LoadDataAsync(SearchBox.Text);
                ApprenticeListChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Opens the edit dialog for the selected apprentice and saves any changes.</summary>
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
                await LoadDataAsync(SearchBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Prompts for confirmation and deletes the selected apprentice including all related data.</summary>
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
                await LoadDataAsync(SearchBox.Text);
                ApprenticeListChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Reloads the apprentice list from the database.</summary>
    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadDataAsync(SearchBox.Text);
    }
}
