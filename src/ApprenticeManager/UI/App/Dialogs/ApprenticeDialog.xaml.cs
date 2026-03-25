using ApprenticeManager.Models;
using System.Windows;

namespace ApprenticeManager.UI.App.Dialogs;

public partial class ApprenticeDialog : Window
{
    public Apprentice Result { get; private set; } = new Apprentice();

    public ApprenticeDialog(Apprentice? existing = null)
    {
        InitializeComponent();
        if (existing != null)
        {
            Title = "Lernenden bearbeiten";
            FirstNameBox.Text = existing.FirstName;
            LastNameBox.Text = existing.LastName;
            DateOfBirthPicker.SelectedDate = existing.DateOfBirth == default
                ? null : existing.DateOfBirth.ToDateTime(TimeOnly.MinValue);
            StartDatePicker.SelectedDate = existing.StartDate == default
                ? null : existing.StartDate.ToDateTime(TimeOnly.MinValue);
            EndDatePicker.SelectedDate = existing.EndDate == default
                ? null : existing.EndDate.ToDateTime(TimeOnly.MinValue);
            OccupationBox.Text = existing.Occupation;
            CompanyBox.Text = existing.Company;
            EmailBox.Text = existing.Email ?? string.Empty;
            PhoneBox.Text = existing.Phone ?? string.Empty;
            NotesBox.Text = existing.Notes ?? string.Empty;
            Result = existing;
        }
        else
        {
            Title = "Lernenden hinzufügen";
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstNameBox.Text) || string.IsNullOrWhiteSpace(LastNameBox.Text))
        {
            MessageBox.Show("Vorname und Nachname sind Pflichtfelder.", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!DateOfBirthPicker.SelectedDate.HasValue ||
            !StartDatePicker.SelectedDate.HasValue ||
            !EndDatePicker.SelectedDate.HasValue)
        {
            MessageBox.Show("Alle Datumsfelder müssen ausgefüllt sein.", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(OccupationBox.Text) || string.IsNullOrWhiteSpace(CompanyBox.Text))
        {
            MessageBox.Show("Beruf und Betrieb sind Pflichtfelder.", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result.FirstName = FirstNameBox.Text.Trim();
        Result.LastName = LastNameBox.Text.Trim();
        Result.DateOfBirth = DateOnly.FromDateTime(DateOfBirthPicker.SelectedDate.Value);
        Result.StartDate = DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value);
        Result.EndDate = DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value);
        Result.Occupation = OccupationBox.Text.Trim();
        Result.Company = CompanyBox.Text.Trim();
        Result.Email = string.IsNullOrWhiteSpace(EmailBox.Text) ? null : EmailBox.Text.Trim();
        Result.Phone = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text.Trim();
        Result.Notes = string.IsNullOrWhiteSpace(NotesBox.Text) ? null : NotesBox.Text.Trim();

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
