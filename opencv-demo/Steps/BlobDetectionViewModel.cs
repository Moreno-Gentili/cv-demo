using CommunityToolkit.Mvvm.ComponentModel;
using OpenCVDemo.Models;
using OpenCvSharp;

namespace OpenCVDemo.Steps;

public partial class BlobDetectionViewModel : ObservableObject, IStep
{
    [ObservableProperty]
    public int minThreshold = 1;
    [ObservableProperty]
    public int maxThreshold = 255;
    [ObservableProperty]
    public int minArea = 10;
    [ObservableProperty]
    public int maxArea = 1000;
    [ObservableProperty]
    public int minCircularity = 10;
    [ObservableProperty]
    public int maxCircularity = 1000;
    [ObservableProperty]
    public int minConvexity = 10;
    [ObservableProperty]
    public int maxConvexity = 1000;
    [ObservableProperty]
    public int minInertia = 10;
    [ObservableProperty]
    public int maxInertia = 1000;

    public ProcessResult Process(Mat image, string path, GenePool genes)
    {
        SimpleBlobDetector detector = SimpleBlobDetector.Create(new SimpleBlobDetector.Params {
            FilterByArea = true,
            FilterByCircularity = true,
            FilterByConvexity = true,
            FilterByInertia = true,
            FilterByColor = true,
            BlobColor = 0,
            MinArea = genes[nameof(MinArea)],
            MaxArea = genes[nameof(MaxArea)],
            MinCircularity = genes[nameof(MinCircularity)] / 1000f,
            MaxCircularity = genes[nameof(MaxCircularity)] / 1000f,
            MinConvexity = genes[nameof(MinConvexity)] / 1000f,
            MaxConvexity = genes[nameof(MaxConvexity)] / 1000f,
            MinInertiaRatio = genes[nameof(MinInertia)] / 1000f,
            MaxInertiaRatio = genes[nameof(MaxInertia)] / 1000f,
            MinThreshold = genes[nameof(MinThreshold)],
            MaxThreshold = genes[nameof(MaxThreshold)]
        });

        KeyPoint[] keyPoints = detector.Detect(image);
        Scalar opaqueRed = new Scalar(0, 0, 255, 255);
        Cv2.DrawKeypoints(image, keyPoints, image, opaqueRed, DrawMatchesFlags.DrawRichKeypoints);

        return new(image);
    }

    public int ThresholdMinimum => 1;
    public int ThresholdMaximum => 255;
    public int AreaMinimum => 1;
    public int AreaMaximum => 1000;
    public int CircularityMinimum => 1;
    public int CircularityMaximum => 1000;
    public int ConvexityMinimum => 1;
    public int ConvexityMaximum => 1000;
    public int InertiaMinimum => 1;
    public int InertiaMaximum => 1000;

    public GenePool Genes
    {
        get
        {
            return new GenePool(
                new Gene(nameof(MinThreshold), ThresholdMinimum, ThresholdMaximum, MinThreshold),
                new Gene(nameof(MaxThreshold), ThresholdMinimum, ThresholdMaximum, MaxThreshold),
                new Gene(nameof(MinArea), AreaMinimum, AreaMaximum, MinArea),
                new Gene(nameof(MaxArea), AreaMinimum, AreaMaximum, MaxArea),
                new Gene(nameof(MinCircularity), CircularityMinimum, CircularityMaximum, MinCircularity),
                new Gene(nameof(MaxCircularity), CircularityMinimum, CircularityMaximum, MaxCircularity),
                new Gene(nameof(MinConvexity), ConvexityMinimum, ConvexityMaximum, MinConvexity),
                new Gene(nameof(MaxConvexity), ConvexityMinimum, ConvexityMaximum, MaxConvexity),
                new Gene(nameof(MinInertia), InertiaMinimum, InertiaMaximum, MinInertia),
                new Gene(nameof(MaxInertia), InertiaMinimum, InertiaMaximum, MaxInertia));
        }

        set
        {
            MinArea = value[nameof(MinArea)];
            MaxArea = value[nameof(MaxArea)];
            MinCircularity = value[nameof(MinCircularity)];
            MaxCircularity = value[nameof(MaxCircularity)];
            MinConvexity = value[nameof(MinConvexity)];
            MaxConvexity = value[nameof(MaxConvexity)];
            MinInertia = value[nameof(MinInertia)];
            MaxInertia = value[nameof(MaxInertia)];
            MinThreshold = value[nameof(MinThreshold)];
            MaxThreshold = value[nameof(MaxThreshold)];
        }
    }
}
