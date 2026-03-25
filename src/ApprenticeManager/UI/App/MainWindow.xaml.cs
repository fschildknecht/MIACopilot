using ApprenticeManager.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace ApprenticeManager.UI.App;

/// <summary>Helper item for the role-switch ComboBox.</summary>
internal class RoleSwitchItem
{
    public string Label { get; set; } = string.Empty;
    public int? ApprenticeId { get; set; }
}

/// <summary>Main application window containing the role switch and all tab controls.</summary>
public partial class MainWindow : Window
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AppState _appState;

    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _appState = serviceProvider.GetRequiredService<AppState>();

        ApprenticesTab.Initialize(serviceProvider);
        GradesTab.Initialize(serviceProvider);
        JournalTab.Initialize(serviceProvider);
        TrainersTab.Initialize(serviceProvider);
        CompaniesTab.Initialize(serviceProvider);

        // Refresh role switch whenever an apprentice is added or removed
        ApprenticesTab.ApprenticeListChanged += async (_, _) => await RefreshRoleSwitchAsync(keepSelection: false);

        Loaded += async (_, _) => await RefreshRoleSwitchAsync(keepSelection: false);
    }

    /// <summary>
    /// Rebuilds the role-switch ComboBox with the current apprentice list.
    /// If keepSelection is true the previously selected item is restored.
    /// </summary>
    private async Task RefreshRoleSwitchAsync(bool keepSelection)
    {
        var previousId = (RoleSwitchComboBox.SelectedItem as RoleSwitchItem)?.ApprenticeId;

        using var scope = _serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IApprenticeService>();
        var apprentices = await service.GetAllAsync();

        var items = new System.Collections.Generic.List<RoleSwitchItem>
        {
            new() { Label = "👤 Lehrlingsbetreuer", ApprenticeId = null }
        };
        foreach (var a in apprentices)
            items.Add(new RoleSwitchItem { Label = $"🎓 {a.FullName}", ApprenticeId = a.Id });

        // Suppress the SelectionChanged handler while we repopulate
        RoleSwitchComboBox.SelectionChanged -= RoleSwitchComboBox_SelectionChanged;
        RoleSwitchComboBox.ItemsSource = items;

        if (keepSelection && previousId.HasValue)
            RoleSwitchComboBox.SelectedItem = items.FirstOrDefault(i => i.ApprenticeId == previousId);
        else
            RoleSwitchComboBox.SelectedIndex = 0;

        RoleSwitchComboBox.SelectionChanged += RoleSwitchComboBox_SelectionChanged;

        // Apply the mode for whatever item ended up selected
        ApplySelectedRole();
    }

    /// <summary>Refreshes the apprentice list in the combo box when it is opened.</summary>
    private async void RoleSwitchComboBox_DropDownOpened(object sender, EventArgs e)
    {
        await RefreshRoleSwitchAsync(keepSelection: true);
    }

    /// <summary>Switches the UI mode whenever the user picks a different role.</summary>
    private void RoleSwitchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplySelectedRole();
    }

    /// <summary>Reads the currently selected role-switch item and updates visibility / data accordingly.</summary>
    private void ApplySelectedRole()
    {
        if (RoleSwitchComboBox.SelectedItem is not RoleSwitchItem item) return;

        if (item.ApprenticeId == null)
        {
            // Trainer mode: show all admin tabs, restore selectors in shared tabs
            _appState.SetVocationalTrainerMode();
            ApprenticesTabItem.Visibility = Visibility.Visible;
            TrainersTabItem.Visibility = Visibility.Visible;
            CompaniesTabItem.Visibility = Visibility.Visible;

            _ = GradesTab.SetTrainerMode();
            _ = JournalTab.SetTrainerMode();

            if (MainTabControl.SelectedItem == null ||
                (MainTabControl.SelectedItem as TabItem)?.Visibility == Visibility.Collapsed)
                MainTabControl.SelectedItem = ApprenticesTabItem;
        }
        else
        {
            // Apprentice mode: hide admin tabs, pre-filter shared tabs
            _appState.SetApprenticeMode(item.ApprenticeId.Value);
            ApprenticesTabItem.Visibility = Visibility.Collapsed;
            TrainersTabItem.Visibility = Visibility.Collapsed;
            CompaniesTabItem.Visibility = Visibility.Collapsed;

            _ = GradesTab.SetApprentice(item.ApprenticeId.Value);
            _ = JournalTab.SetApprentice(item.ApprenticeId.Value);

            MainTabControl.SelectedItem = JournalTabItem;
        }
    }
}
