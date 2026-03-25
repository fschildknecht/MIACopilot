using ApprenticeManager.Models;
using System.Globalization;
using System.Windows;

namespace ApprenticeManager.UI.App.Dialogs;

public partial class GradeDialog : Window
{
    public decimal GradeValue { get; private set; }
    public DateOnly GradeDate { get; private set; }
    public string? GradeNotes { get; private set; }

    public GradeDialog(Grade? existing = null)
    {
        InitializeComponent();
        if (existing != null)
        {
            Title = "Note bearbeiten";
            GradeValueBox.Text = existing.Value.ToString("F1", CultureInfo.CurrentCulture);
            GradeDatePicker.SelectedDate = existing.Date.ToDateTime(TimeOnly.MinValue);
            GradeNotesBox.Text = existing.Notes ?? string.Empty;
        }
        else
        {
            Title = "Note erfassen";
            GradeDatePicker.SelectedDate = DateTime.Today;
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        var valueText = GradeValueBox.Text.Replace(',', '.');
        if (!decimal.TryParse(valueText, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)
            || value < 1.0m || value > 6.0m)
        {
            MessageBox.Show("Die Note muss zwischen 1.0 und 6.0 liegen.", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!GradeDatePicker.SelectedDate.HasValue)
        {
            MessageBox.Show("Das Datum ist ein Pflichtfeld.", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        GradeValue = value;
        GradeDate = DateOnly.FromDateTime(GradeDatePicker.SelectedDate.Value);
        GradeNotes = string.IsNullOrWhiteSpace(GradeNotesBox.Text) ? null : GradeNotesBox.Text.Trim();
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
