namespace ApprenticeManager.Services;

/// <summary>Describes which role the current user is operating as.</summary>
public enum AppViewMode { VocationalTrainer, Apprentice }

/// <summary>
/// Singleton service that tracks the active view mode and the selected apprentice.
/// Controls subscribe to <see cref="ViewModeChanged"/> to react to role switches.
/// </summary>
public class AppState
{
    /// <summary>The currently active view mode.</summary>
    public AppViewMode ViewMode { get; private set; } = AppViewMode.VocationalTrainer;

    /// <summary>ID of the apprentice whose data is shown in apprentice mode; null in trainer mode.</summary>
    public int? ActiveApprenticeId { get; private set; }

    /// <summary>Raised whenever the view mode or active apprentice changes.</summary>
    public event EventHandler? ViewModeChanged;

    /// <summary>Switches to full-access vocational trainer mode.</summary>
    public void SetVocationalTrainerMode()
    {
        ViewMode = AppViewMode.VocationalTrainer;
        ActiveApprenticeId = null;
        ViewModeChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Switches to apprentice mode scoped to the given apprentice ID.</summary>
    public void SetApprenticeMode(int apprenticeId)
    {
        ViewMode = AppViewMode.Apprentice;
        ActiveApprenticeId = apprenticeId;
        ViewModeChanged?.Invoke(this, EventArgs.Empty);
    }
}
