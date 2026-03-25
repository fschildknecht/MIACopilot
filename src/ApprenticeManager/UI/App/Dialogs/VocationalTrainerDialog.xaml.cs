using ApprenticeManager.Models;
using System.Windows;

namespace ApprenticeManager.UI.App.Dialogs;

/// <summary>Dialog for creating or editing a vocational trainer.</summary>
public partial class VocationalTrainerDialog : Window
{
    public VocationalTrainer Result { get; private set; } = new VocationalTrainer();

    /// <summary>Opens the dialog; if an existing trainer is passed, the fields are pre-filled for editing.</summary>
    public VocationalTrainerDialog(VocationalTrainer? existing = null)
    {
        InitializeComponent();
        if (existing != null)
        {
            Title = "Lehrlingsbetreuer bearbeiten";
            FirstNameBox.Text = existing.FirstName;
            LastNameBox.Text = existing.LastName;
            CompanyBox.Text = existing.Company;
            EmailBox.Text = existing.Email ?? string.Empty;
            PhoneBox.Text = existing.Phone ?? string.Empty;
            NotesBox.Text = existing.Notes ?? string.Empty;
            Result = existing;
        }
        else
        {
            Title = "Lehrlingsbetreuer hinzufügen";
        }
    }

    /// <summary>Validates input and closes the dialog with DialogResult = true on success.</summary>
    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstNameBox.Text) || string.IsNullOrWhiteSpace(LastNameBox.Text))
        {
            MessageBox.Show("Vorname und Nachname sind Pflichtfelder.", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(CompanyBox.Text))
        {
            MessageBox.Show("Betrieb ist ein Pflichtfeld.", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result.FirstName = FirstNameBox.Text.Trim();
        Result.LastName = LastNameBox.Text.Trim();
        Result.Company = CompanyBox.Text.Trim();
        Result.Email = string.IsNullOrWhiteSpace(EmailBox.Text) ? null : EmailBox.Text.Trim();
        Result.Phone = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text.Trim();
        Result.Notes = string.IsNullOrWhiteSpace(NotesBox.Text) ? null : NotesBox.Text.Trim();

        DialogResult = true;
    }

    /// <summary>Closes the dialog without saving.</summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
