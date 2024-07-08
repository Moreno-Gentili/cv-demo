using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OpenCVDemo.Components;

/// <summary>
/// Interaction logic for NumericStepper.xaml
/// </summary>
public partial class NumericStepper : UserControl
{
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(int),
        typeof(NumericStepper));

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum), typeof(int),
        typeof(NumericStepper));

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum), typeof(int),
        typeof(NumericStepper));

    public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
        nameof(Step), typeof(int),
        typeof(NumericStepper));

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public int Step
    {
        get => (int)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    private readonly DispatcherTimer _timer;

    public NumericStepper()
    {
        InitializeComponent();
        _timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(200), IsEnabled = false };
    }

    private void StartAdding(object sender, MouseButtonEventArgs e)
    {
        _timer.Tick += Add;
        _timer.IsEnabled = true;
        Add(null, null);
    }

    private void StopAdding(object sender, MouseButtonEventArgs e)
    {
        _timer.Tick -= Add;
        _timer.IsEnabled = false;
    }

    private void StartSubtracting(object sender, MouseButtonEventArgs e)
    {
        _timer.Tick += Subtract;
        _timer.IsEnabled = true;
        Subtract(null, null);
    }

    private void StopSubtracting(object sender, MouseButtonEventArgs e)
    {
        _timer.Tick -= Subtract;
        _timer.IsEnabled = false;
    }

    private void Add(object? sender, EventArgs? args)
    {
        Value = Math.Max(Minimum, Math.Min(Maximum, Value + Step));
    }

    private void Subtract(object? sender, EventArgs? args)
    {
        Value = Math.Max(Minimum, Math.Min(Maximum, Value - Step));
    }
}
