using System.Windows;
using System.Windows.Media;

namespace OpenAIDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
        FontFamily = new FontFamily("Barlow Condensed");
    }
}