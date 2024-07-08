using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenCVDemo.Components
{
    /// <summary>
    /// Interaction logic for GeneticProgress.xaml
    /// </summary>
    public partial class GeneticProgress : UserControl
    {
        public static readonly DependencyProperty StartCommandProperty = DependencyProperty.Register(
        nameof(StartCommand), typeof(ICommand),
        typeof(GeneticProgress));

        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(
        nameof(CancelCommand), typeof(ICommand),
        typeof(GeneticProgress));

        public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(
        nameof(CloseCommand), typeof(ICommand),
        typeof(GeneticProgress));

        public static readonly DependencyProperty GenerationsProperty = DependencyProperty.Register(
        nameof(Generations), typeof(int),
        typeof(GeneticProgress));

        public static readonly DependencyProperty CurrentGenerationProperty = DependencyProperty.Register(
        nameof(CurrentGeneration), typeof(int),
        typeof(GeneticProgress));

        public static readonly DependencyProperty FitnessProperty = DependencyProperty.Register(
        nameof(Fitness), typeof(double?[]),
        typeof(GeneticProgress), new PropertyMetadata() { PropertyChangedCallback = OnUpdateFitness });

        public static readonly DependencyProperty FitnessGeometryProperty = DependencyProperty.Register(
        nameof(FitnessGeometry), typeof(Geometry),
        typeof(GeneticProgress));

        public static readonly DependencyProperty TopFitnessProperty = DependencyProperty.Register(
        nameof(TopFitness), typeof(double?),
        typeof(GeneticProgress));

        public ICommand? StartCommand
        {
            get => (ICommand?)GetValue(StartCommandProperty);
            set => SetValue(StartCommandProperty, value);
        }

        public ICommand? CancelCommand
        {
            get => (ICommand?)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }

        public ICommand? CloseCommand
        {
            get => (ICommand?)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public int? Generations
        {
            get => (int?)GetValue(GenerationsProperty);
            set => SetValue(GenerationsProperty, value);
        }

        public int? CurrentGeneration
        {
            get => (int?)GetValue(CurrentGenerationProperty);
            set => SetValue(CurrentGenerationProperty, value);
        }

        public double?[]? Fitness
        {
            get => (double?[]?)GetValue(FitnessProperty);
            set => SetValue(FitnessProperty, value);
        }

        public double? TopFitness
        {
            get => (double?)GetValue(TopFitnessProperty);
            set => SetValue(TopFitnessProperty, value);
        }

        public Geometry? FitnessGeometry
        {
            get => (Geometry?)GetValue(FitnessGeometryProperty);
            set => SetValue(FitnessGeometryProperty, value);
        }

        private static void OnUpdateFitness(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double?[]? fitness = (double?[]?)e.NewValue;
            if (fitness is null)
            {
                return;
            }

            GeneticProgress progress = (GeneticProgress)d;
            int generations = progress.Generations ?? 100;
            double w = progress.FitnessChart.ActualWidth / generations;
            double h = progress.FitnessChart.ActualHeight - 1;

            StringBuilder sb = new();
            sb.Append($"M0,{h.ToString(CultureInfo.InvariantCulture)}");

            for (int i = 0; i < fitness.Length; i++)
            {
                double? f = fitness[i];
                if (!f.HasValue)
                {
                    break;
                }

                var y = h - (f.Value * h);
                var x = (i + 1) * w;
                sb.Append($" L{x.ToString(CultureInfo.InvariantCulture)},{y.ToString(CultureInfo.InvariantCulture)}");
            }

            progress.FitnessGeometry = Geometry.Parse(sb.ToString());
        }

        public GeneticProgress()
        {
            InitializeComponent();
        }
    }
}
