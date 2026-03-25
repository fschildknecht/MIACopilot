using ApprenticeManager.Models;
using System.Globalization;
using System.Windows;

namespace ApprenticeManager.UI.App.Dialogs;

public partial class JournalDialog : Window
{
    public LearningJournal Result { get; private set; } = new LearningJournal();

    public JournalDialog(LearningJournal? existing = null)
    {
        InitializeComponent();
        if (existing != null)
        {
            Title = "Journaleintrag bearbeiten";
            KWBox.Text = existing.Kalenderwoche.ToString();
            JahrBox.Text = existing.Jahr.ToString();
            BetriebBox.Text = existing.BetrieblicheTaetigkeiten;
            SchuleBox.Text = existing.SchulischeTaetigkeiten;
            ReflexionBox.Text = existing.Reflexion;
            Result = existing;
        }
        else
        {
            Title = "Neuer Journaleintrag";
            var cal = CultureInfo.CurrentCulture.Calendar;
            KWBox.Text = cal.GetWeekOfYear(DateTime.Today,
                CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString();
            JahrBox.Text = DateTime.Today.Year.ToString();
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(KWBox.Text, out var kw) || kw < 1 || kw > 53)
        {
            MessageBox.Show("Die Kalenderwoche muss zwischen 1 und 53 liegen.", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(JahrBox.Text, out var jahr) || jahr < 2000 || jahr > 2100)
        {
            MessageBox.Show("Bitte ein gültiges Jahr eingeben (2000–2100).", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(BetriebBox.Text))
        {
            MessageBox.Show("Betriebliche Tätigkeiten sind ein Pflichtfeld.", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result.Kalenderwoche = kw;
        Result.Jahr = jahr;
        Result.BetrieblicheTaetigkeiten = BetriebBox.Text.Trim();
        Result.SchulischeTaetigkeiten = SchuleBox.Text.Trim();
        Result.Reflexion = ReflexionBox.Text.Trim();
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
