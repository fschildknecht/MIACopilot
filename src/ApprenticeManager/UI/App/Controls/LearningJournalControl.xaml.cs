using ApprenticeManager.Models;
using ApprenticeManager.Services;
using ApprenticeManager.UI.App.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace ApprenticeManager.UI.App.Controls;

/// <summary>UserControl that shows a master-detail view of an apprentice's learning journal.</summary>
public partial class LearningJournalControl : UserControl
{
    private IServiceProvider? _serviceProvider;
    private int? _activeApprenticeId;

    public LearningJournalControl()
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
    /// Hides the apprentice selector and loads that apprentice's journal immediately.
    /// </summary>
    public async Task SetApprentice(int apprenticeId)
    {
        _activeApprenticeId = apprenticeId;
        ApprenticeSelector.Visibility = Visibility.Collapsed;
        await LoadJournalAsync(apprenticeId);
    }

    /// <summary>Restores the trainer-mode view with the apprentice selector visible.</summary>
    public async Task SetTrainerMode()
    {
        _activeApprenticeId = null;
        ApprenticeSelector.Visibility = Visibility.Visible;
        JournalList.ItemsSource = null;
        ClearDetailPanel();
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
            JournalList.ItemsSource = null;
            ClearDetailPanel();
            return;
        }
        _activeApprenticeId = apprentice.Id;
        await LoadJournalAsync(apprentice.Id);
    }

    /// <summary>Loads all journal entries for the given apprentice and binds them to the list.</summary>
    private async Task LoadJournalAsync(int apprenticeId)
    {
        using var scope = _serviceProvider!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILearningJournalService>();
        var entries = await service.GetByApprenticeAsync(apprenticeId);
        JournalList.ItemsSource = entries.ToList();
        ClearDetailPanel();
    }

    /// <summary>Updates the detail panel when a journal entry is selected in the list.</summary>
    private void JournalList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (JournalList.SelectedItem is not LearningJournal entry)
        {
            ClearDetailPanel();
            return;
        }
        ShowDetailPanel(entry);
    }

    /// <summary>Populates the detail panel with the full content of the given journal entry.</summary>
    private void ShowDetailPanel(LearningJournal entry)
    {
        EmptyHint.Visibility = Visibility.Collapsed;
        DetailPanel.Visibility = Visibility.Visible;

        DetailTitle.Text = entry.Titel;
        var geaendert = entry.GeaendertAm.HasValue
            ? $"  |  Geändert: {entry.GeaendertAm.Value:dd.MM.yyyy}"
            : string.Empty;
        DetailMeta.Text = $"Erstellt: {entry.ErstelltAm:dd.MM.yyyy}{geaendert}";
        DetailBetrieb.Text = string.IsNullOrWhiteSpace(entry.BetrieblicheTaetigkeiten)
            ? "–" : entry.BetrieblicheTaetigkeiten;
        DetailSchule.Text = string.IsNullOrWhiteSpace(entry.SchulischeTaetigkeiten)
            ? "–" : entry.SchulischeTaetigkeiten;
        DetailReflexion.Text = string.IsNullOrWhiteSpace(entry.Reflexion)
            ? "–" : entry.Reflexion;
    }

    /// <summary>Resets the detail panel to the empty-hint state.</summary>
    private void ClearDetailPanel()
    {
        DetailPanel.Visibility = Visibility.Collapsed;
        EmptyHint.Visibility = Visibility.Visible;
    }

    /// <summary>Opens the add dialog and saves the new journal entry if confirmed.</summary>
    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (_activeApprenticeId == null)
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
                dialog.Result.ApprenticeId = _activeApprenticeId.Value;
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ILearningJournalService>();
                await service.AddAsync(dialog.Result);
                await LoadJournalAsync(_activeApprenticeId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Opens the edit dialog for the selected entry and saves any changes.</summary>
    private async void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (JournalList.SelectedItem is not LearningJournal selected) return;
        if (_activeApprenticeId == null) return;

        var dialog = new JournalDialog(selected) { Owner = Window.GetWindow(this) };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ILearningJournalService>();
                await service.UpdateAsync(dialog.Result);
                await LoadJournalAsync(_activeApprenticeId.Value);
                ShowDetailPanel(dialog.Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Prompts for confirmation and deletes the selected journal entry.</summary>
    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (JournalList.SelectedItem is not LearningJournal selected) return;
        if (_activeApprenticeId == null) return;

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
                await LoadJournalAsync(_activeApprenticeId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>Reloads the journal entries for the currently active apprentice.</summary>
    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        if (_activeApprenticeId.HasValue)
            await LoadJournalAsync(_activeApprenticeId.Value);
    }
}
