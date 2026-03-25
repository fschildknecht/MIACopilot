using System.Windows;

namespace ApprenticeManager.UI.App.Dialogs;

public partial class SubjectDialog : Window
{
    public string SubjectName { get; private set; } = string.Empty;

    public SubjectDialog()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            MessageBox.Show("Fachname ist ein Pflichtfeld.", "Validierungsfehler",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        SubjectName = NameBox.Text.Trim();
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
