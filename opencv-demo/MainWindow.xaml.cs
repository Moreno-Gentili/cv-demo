using System.Windows.Media;

namespace OpenCVDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : System.Windows.Window
{
    public MainWindow(MainWindowViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
        FontFamily = new FontFamily("Barlow Condensed");
    }
}