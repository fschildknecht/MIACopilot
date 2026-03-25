using System.Windows;

namespace ApprenticeManager.UI.App;

public partial class MainWindow : Window
{
    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        ApprenticesTab.Initialize(serviceProvider);
        GradesTab.Initialize(serviceProvider);
        JournalTab.Initialize(serviceProvider);
    }
}
