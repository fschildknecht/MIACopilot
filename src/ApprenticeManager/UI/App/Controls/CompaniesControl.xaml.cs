using ApprenticeManager.Models;
using ApprenticeManager.Services;
using ApprenticeManager.UI.App.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace ApprenticeManager.UI.App.Controls;

/// <summary>UserControl that lists all companies and provides CRUD actions.</summary>
public partial class CompaniesControl : UserControl
{
    private IServiceProvider? _serviceProvider;

    public CompaniesControl()
    {
        InitializeComponent();
    }

    /// <summary>Injects the DI container and triggers the first data load once the control is ready.</summary>
    public void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Loaded += async (_, _) => await LoadDataAsync();
    }

    /// <summary>Loads all companies from the database and binds them to the grid.</summary>
    private async Task LoadDataAsync()
    {
        using var scope = _serviceProvider!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ICompanyService>();
        var companies = await service.GetAllAsync();
        CompaniesGrid.ItemsSource = companies.ToList();
    }

    /// <summary>Opens the add dialog and persists the new company if confirmed.</summary>
    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new CompanyDialog { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ICompanyService>();
                await service.AddAsync(dialog.Result);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Opens the edit dialog for the selected company and saves any changes.</summary>
    private async void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (CompaniesGrid.SelectedItem is not Company selected) return;

        var dialog = new CompanyDialog(selected) { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ICompanyService>();
                await service.UpdateAsync(dialog.Result);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Prompts for confirmation and deletes the selected company.</summary>
    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (CompaniesGrid.SelectedItem is not Company selected) return;

        var confirm = MessageBox.Show(
            $"Betrieb '{selected.Name}' wirklich löschen?",
            "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (confirm == MessageBoxResult.Yes)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ICompanyService>();
                await service.DeleteAsync(selected.Id);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Reloads the company list from the database.</summary>
    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadDataAsync();
    }
}
