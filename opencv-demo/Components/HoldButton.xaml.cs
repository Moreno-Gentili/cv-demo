using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenCVDemo.Components
{
    /// <summary>
    /// Interaction logic for HoldButton.xaml
    /// </summary>
    public partial class HoldButton : UserControl
    {
        public static readonly DependencyProperty PressCommandProperty = DependencyProperty.Register(
        nameof(PressCommand), typeof(ICommand),
        typeof(HoldButton));

        public static readonly DependencyProperty ReleaseCommandProperty = DependencyProperty.Register(
        nameof(ReleaseCommand), typeof(ICommand),
        typeof(HoldButton));

        public HoldButton()
        {
            InitializeComponent();
        }

        public ICommand? PressCommand
        {
            get => (ICommand?)GetValue(PressCommandProperty);
            set => SetValue(PressCommandProperty, value);
        }

        public ICommand? ReleaseCommand
        {
            get => (ICommand?)GetValue(ReleaseCommandProperty);
            set => SetValue(ReleaseCommandProperty, value);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PressCommand is null)
            {
                return;
            }

            if (PressCommand.CanExecute(null))
            {
                PressCommand.Execute(null);
            }
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ReleaseCommand is null)
            {
                return;
            }

            if (ReleaseCommand.CanExecute(null))
            {
                ReleaseCommand.Execute(null);
            }
        }
    }
}
