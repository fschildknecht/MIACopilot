using ApprenticeManager.Models;
using System.Windows;

namespace ApprenticeManager.UI.App.Dialogs;

/// <summary>Dialog for creating or editing a company record.</summary>
public partial class CompanyDialog : Window
{
    public Company Result { get; private set; } = new Company();

    /// <summary>Opens the dialog; if an existing company is passed, the fields are pre-filled for editing.</summary>
    public CompanyDialog(Company? existing = null)
    {
        InitializeComponent();
        if (existing != null)
        {
            Title = "Betrieb bearbeiten";
            NameBox.Text = existing.Name;
            AddressBox.Text = existing.Address ?? string.Empty;
            PhoneBox.Text = existing.Phone ?? string.Empty;
            EmailBox.Text = existing.Email ?? string.Empty;
            NotesBox.Text = existing.Notes ?? string.Empty;
            Result = existing;
        }
        else
        {
            Title = "Betrieb hinzufügen";
        }
    }

    /// <summary>Validates input and closes the dialog with DialogResult = true on success.</summary>
    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            MessageBox.Show("Firmenname ist ein Pflichtfeld.", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result.Name = NameBox.Text.Trim();
        Result.Address = string.IsNullOrWhiteSpace(AddressBox.Text) ? null : AddressBox.Text.Trim();
        Result.Phone = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text.Trim();
        Result.Email = string.IsNullOrWhiteSpace(EmailBox.Text) ? null : EmailBox.Text.Trim();
        Result.Notes = string.IsNullOrWhiteSpace(NotesBox.Text) ? null : NotesBox.Text.Trim();

        DialogResult = true;
    }

    /// <summary>Closes the dialog without saving.</summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
