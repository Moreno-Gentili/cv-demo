using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneticSharp;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using OpenCVDemo.Models;
using OpenCvSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Rect = System.Windows.Rect;
namespace OpenCVDemo;

public partial class MainWindowViewModel : ObservableObject, IDropTarget
{
    readonly BroadcastBlock<MainWindowViewModel> _processBlock = new(t => t);
    readonly ActionBlock<MainWindowViewModel> _fitBlock;
    readonly IServiceProvider _serviceProvider;
    CancellationTokenSource _fitCancellationTokenSource = new();

    public MainWindowViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _processBlock.LinkTo(new ActionBlock<MainWindowViewModel>(Process, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 }));
        _fitBlock = new ActionBlock<MainWindowViewModel>(Fit);
        FolderPath = GetDatasetDirectory();
    }

    [ObservableProperty]
    bool isAddStepMenuOpen = false;

    [ObservableProperty]
    bool canProcess = false;

    [ObservableProperty]
    string[] images = [];

    [ObservableProperty]
    Rect[] labels = [];

    [ObservableProperty]
    int selectedImageIndex = 0;

    [ObservableProperty]
    int maxImageIndex = 0;

    [ObservableProperty]
    string folderPath = string.Empty;

    [ObservableProperty]
    ImageSource? originalImage;

    [ObservableProperty]
    ImageSource? transformedImage;

    [ObservableProperty]
    ImageSource? annotatedImage;

    [ObservableProperty]
    int duration;

    [ObservableProperty]
    bool isOriginalImageRevealed;

    [ObservableProperty]
    bool hasLabeling;

    [ObservableProperty]
    bool hasFitting;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartFittingCommand), nameof(CancelFittingCommand))]
    bool isFitting;

    [ObservableProperty]
    Rect? selectedImageLabel;

    [ObservableProperty]
    string? selectedImageName;

    [ObservableProperty]
    int currentGeneration;

    [ObservableProperty]
    double? topFitness;

    [ObservableProperty]
    double?[] fitness = [];

    [ObservableProperty]
    List<GenePool>? topGenePools;

    public ObservableCollection<BaseStepViewModel> Steps { get; } = new();

    public CancellationToken FitToken => _fitCancellationTokenSource.Token;


    string GetDatasetDirectory() => Path.Combine(AppContext.BaseDirectory, "Content/Dataset");

    partial void OnFolderPathChanged(string value)
    {
        SelectedImageIndex = int.MinValue;
        MaxImageIndex = int.MinValue;
        Labels = [];

        if (Directory.Exists(value))
        {
            Images = Directory.EnumerateFiles(value).Where(f => Path.GetExtension(f).ToLower().Trim('.') is "jpg" or "jpeg" or "png").ToArray();
        }
        else
        {
            Images = [];
        }
    }

    partial void OnImagesChanged(string[] value)
    {
        CanProcess = value.Length > 0;
        if (CanProcess)
        {
            Labels = value.Select(LoadLabel).ToArray();
            MaxImageIndex = value.Length - 1;
            SelectedImageIndex = 0;
        }
    }

    partial void OnSelectedImageIndexChanged(int value)
    {
        if (value >= 0 && value < Labels.Length)
        {
            SelectedImageLabel = Labels[value];
            SelectedImageName = Path.GetFileName(Images[value]);
        }

        StartProcessing();
    }


    [RelayCommand]
    void AddStep(string stepName)
    {
        AddStep(stepName, true, null);
    }

    void AddStep(string stepName, bool isEnabled, GenePool? genes)
    {
        IsAddStepMenuOpen = false;
        Type stepType = Type.GetType($"{GetType().Namespace}.Steps.{stepName}") ??
                        throw new InvalidOperationException($"Impossibile trovare lo step '{stepName}'");

        BaseStep step = _serviceProvider.GetRequiredService(stepType) as BaseStep ??
                        throw new InvalidOperationException($"Impossibile creare lo step '{stepName}'"); ;

        BaseStepViewModel vm = new(step, DeleteCommand);
        vm.IsEnabled = isEnabled;
        if (genes is not null)
        {
            vm.Genes = genes;
        }

        vm.PropertyChanged += HandlePropertyChanged;

        Steps.Add(vm);
        StartProcessing();
    }

    [RelayCommand]
    void ToggleAddStepMenu()
    {
        IsAddStepMenuOpen = !IsAddStepMenuOpen;
    }

    [RelayCommand]
    void Browse()
    {
        OpenFolderDialog ofd = new OpenFolderDialog();
        if (ofd.ShowDialog() == true)
        {
            FolderPath = ofd.FolderName;
        }
    }

    [RelayCommand]
    void Delete(BaseStepViewModel step)
    {
        step.PropertyChanged -= HandlePropertyChanged;
        Steps.Remove(step);
        StartProcessing();
    }

    [RelayCommand]
    void RevealOriginalImage()
    {
        IsOriginalImageRevealed = true;
    }

    [RelayCommand]
    void HideOriginalImage()
    {
        IsOriginalImageRevealed = false;
    }

    #region processing

    void StartProcessing()
    {
        if (Images.Length > 0 && SelectedImageIndex >= 0 && SelectedImageIndex <= MaxImageIndex)
        {
            _processBlock.Post(this);
        }
    }

    static void Process(MainWindowViewModel viewModel)
    {
        (Mat originalImage, string path) = DecodeOriginalImage(viewModel, viewModel.SelectedImageIndex);

        ImageSource? originalImageSource = ToBitmapSource(originalImage);
        List<Action> sideEffects = new();

        long startedAt = TimeProvider.System.GetTimestamp();
        Mat transformedImage = originalImage;
        Mat? annotatedImage = null;
        foreach (IStep step in viewModel.Steps.ToList())
        {
            ProcessResult result = step.Process(transformedImage, path);
            transformedImage = result.TransformedImage;
            annotatedImage ??= result.Annotations;

            if (result.SideEffects is not null)
            {
                sideEffects.Add(result.SideEffects);
            }
        }

        int duration = Convert.ToInt32(TimeProvider.System.GetElapsedTime(startedAt).TotalMilliseconds);
        ImageSource? transformedImageSource = ToBitmapSource(transformedImage);
        ImageSource? annotatedImageSource = ToBitmapSource(annotatedImage);

        Application.Current.Dispatcher.Invoke(() =>
        {
            viewModel.TransformedImage = transformedImageSource;
            viewModel.OriginalImage = originalImageSource;
            viewModel.AnnotatedImage = annotatedImageSource;
            viewModel.Duration = duration;

            foreach (Action sideEffect in sideEffects)
            {
                sideEffect.Invoke();
            }
        });
    }

    void HandlePropertyChanged(object? sender, PropertyChangedEventArgs p)
    {
        StartProcessing();
    }
    #endregion

    #region labeling

    [RelayCommand]
    void SaveLabel(Rect rect)
    {
        if (SelectedImageIndex < 0)
        {
            return;
        }

        string labelPath = GetLabelPath(Images[SelectedImageIndex]);
        Labels[SelectedImageIndex] = rect;
        File.WriteAllText(labelPath, $"{rect.Left.ToString(CultureInfo.InvariantCulture)} {rect.Top.ToString(CultureInfo.InvariantCulture)} {rect.Width.ToString(CultureInfo.InvariantCulture)} {rect.Height.ToString(CultureInfo.InvariantCulture)}");
        SelectedImageLabel = rect;
    }

    Rect LoadLabel(string path)
    {
        string labelPath = GetLabelPath(path);
        try
        {
            string[] values = File.ReadAllText(labelPath).Trim().Split(' ');
            return new Rect(
                double.Parse(values[0], CultureInfo.InvariantCulture),
                double.Parse(values[1], CultureInfo.InvariantCulture),
                double.Parse(values[2], CultureInfo.InvariantCulture),
                double.Parse(values[3], CultureInfo.InvariantCulture));
        }
        catch
        {
            Rect rect = new(0.25, 0.25, 0.5, 0.5);
            SaveLabel(rect);
            return rect;
        }
    }

    static string GetLabelPath(string path)
    {
        return $"{path}.txt";
    }
    #endregion

    #region fitting

    [RelayCommand(CanExecute = nameof(CanStartFitting))]
    void StartFitting()
    {
        if (Labels.Any(l => l.Width == 0 || l.Height == 0))
        {
            MessageBox.Show("Impossibile avviare la ricerca: non tutte le immagini sono state etichettate");
            return;
        }

        TopGenePools = null;
        TopFitness = null;
        IsFitting = true;
        CurrentGeneration = 0;
        Fitness = Enumerable.Repeat((double?)null, Generations).ToArray();

        _fitCancellationTokenSource = new();
        _fitBlock.Post(this);
    }

    bool CanStartFitting()
    {
        return !IsFitting;
    }

    [RelayCommand(CanExecute = nameof(CanCancelFitting))]
    void CancelFitting()
    {
        IsFitting = false;
        if (!_fitCancellationTokenSource.IsCancellationRequested)
        {
            _fitCancellationTokenSource.Cancel();
        }
    }

    bool CanCancelFitting()
    {
        return IsFitting;
    }

    [RelayCommand]
    void CloseFitting()
    {
        CancelFitting();
        HasFitting = false;
        if (TopGenePools is not null)
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].Genes = TopGenePools[i];
            }
        }

        TopGenePools = null;
        TopFitness = null;
    }


    public int Generations => 20;
    static void Fit(MainWindowViewModel vm)
    {
        // https://diegogiacomelli.com.br/function-optimization-with-geneticsharp/
        var genes = vm.Steps.SelectMany(s => s.Genes).ToList();
        FloatingPointChromosome chromosome = new(
            minValue: genes.Select(g => Convert.ToDouble(g.Min)).ToArray(),
            maxValue: genes.Select(g => Convert.ToDouble(g.Max)).ToArray(),
            totalBits: genes.Select(g => g.Bits).ToArray(),
            fractionDigits: genes.Select(g => 0).ToArray());

        TplPopulation population = new(minSize: 40, maxSize: 40, chromosome);
        EliteSelection selection = new();
        UniformCrossover crossover = new(0.5f);
        FlipBitMutation mutation = new();
        GeneticFitness fitness = new(vm, CalculateFitness);
        GeneticTermination termination = new(vm, ReportEvolution);

        GeneticAlgorithm evolution = new(population, fitness, selection, crossover, mutation);
        evolution.Termination = termination;
        evolution.Start();

        Application.Current.Dispatcher.Invoke(() =>
        {
            vm.TopFitness = evolution.BestChromosome.Fitness;
            vm.TopGenePools = fitness.ChromosomeToGenePools(evolution.BestChromosome);
            vm.IsFitting = false;
        });
    }

    static void ReportEvolution(MainWindowViewModel vm, int generation, double fitness)
    {
        if (vm.TopFitness is null || vm.TopFitness.Value < fitness || vm.CurrentGeneration < generation)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                vm.TopFitness = fitness;
                vm.CurrentGeneration = generation;

                vm.Fitness[generation - 1] = fitness;
                vm.Fitness = (double?[])vm.Fitness.Clone();
            });
        }
    }

    static double CalculateFitness(IReadOnlyList<GenePool> genePools, IReadOnlyList<IStep> steps, MainWindowViewModel vm)
    {
        List<double> fitnessList = new();
        for (int i = 0; i < vm.Images.Length; i++)
        {
            (Mat originalImage, string path) = DecodeOriginalImage(vm, i);
            Rect label = vm.Labels[i];
            if (label.Width == 0 || label.Height == 0)
            {
                return 0d;
            }

            label = new Rect(label.Left * originalImage.Width, label.Top * originalImage.Height, label.Width * originalImage.Width, label.Height * originalImage.Height);

            Mat transformedImage = originalImage;
            Rect? prediction = null;

            for (int s = 0; s < steps.Count; s++)
            {
                ProcessResult result = steps[s].Process(transformedImage, path, genePools[s]);
                transformedImage = result.TransformedImage;
                prediction ??= result.Boundary;
            }

            if (prediction is null)
            {
                return 0d;
            }

            if (prediction.Value.Left > label.Left ||
                prediction.Value.Right < label.Right ||
                prediction.Value.Top > label.Top ||
                prediction.Value.Bottom < label.Bottom)
            {
                return 0d;
            }

            double labelArea = label.Width * label.Height;
            double predictionArea = prediction.Value.Width * prediction.Value.Height;
            double difference = (predictionArea / labelArea) - 1d;

            double fitness = 1.0d - Math.Atanh(difference);
            fitnessList.Add(fitness);
        }

        return fitnessList.Average();
    }
    #endregion

    #region helpers
    static (Mat, string) DecodeOriginalImage(MainWindowViewModel viewModel, int index)
    {
        string path = viewModel.Images[index];
        return (new Mat(path, ImreadModes.Unchanged), path);
    }

    static ImageSource? ToBitmapSource(Mat? image)
    {
        if (image is null)
        {
            return null;
        }

        byte[] imageData = image.ImEncode(".png");
        if (imageData == null || imageData.Length == 0) return null;

        var bitmapImage = new BitmapImage();
        using (var mem = new MemoryStream(imageData))
        {
            mem.Position = 0;
            bitmapImage.BeginInit();
            bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = null;
            bitmapImage.StreamSource = mem;
            bitmapImage.EndInit();
        }

        bitmapImage.Freeze();
        return bitmapImage ?? throw new InvalidOperationException("Non è stato possibile produrre un'immagine");
    }
    #endregion

    #region IDropTarget implementation
    public void DragOver(IDropInfo dropInfo)
    {
        BaseStepViewModel? sourceItem = dropInfo.Data as BaseStepViewModel;
        BaseStepViewModel? targetItem = dropInfo.TargetItem as BaseStepViewModel;

        if (sourceItem != null && targetItem != null && Steps != null)
        {
            int oldIndex = Steps.IndexOf(sourceItem);
            int newIndex = GetMoveIndex(oldIndex, dropInfo.InsertIndex);
            if (oldIndex != newIndex)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        BaseStepViewModel? sourceItem = dropInfo.Data as BaseStepViewModel;
        BaseStepViewModel? targetItem = dropInfo.TargetItem as BaseStepViewModel;
        if (sourceItem != null && targetItem != null && Steps != null)
        {
            int oldIndex = Steps.IndexOf(sourceItem);
            int newIndex = GetMoveIndex(oldIndex, dropInfo.InsertIndex);
            if (oldIndex != newIndex)
            {
                Steps.Move(oldIndex, newIndex);
                StartProcessing();
            }
        }
    }

    private int GetMoveIndex(int oldIndex, int insertIndex)
    {
        int moveIndex = insertIndex;
        if (oldIndex < moveIndex)
        {
            moveIndex--;
        }

        return moveIndex;
    }
    #endregion
}
