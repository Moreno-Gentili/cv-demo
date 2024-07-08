using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace OpenCVDemo.Components
{
    /// <summary>
    /// Interaction logic for StepMenuItem.xaml
    /// </summary>
    public partial class StepMenuItem : Button
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string),
            typeof(StepMenuItem));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon), typeof(string),
            typeof(StepMenuItem));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public StepMenuItem()
        {
            InitializeComponent();
        }
    }
}
