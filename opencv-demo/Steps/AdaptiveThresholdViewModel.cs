using CommunityToolkit.Mvvm.ComponentModel;
using OpenCVDemo.Models;
using OpenCvSharp;

namespace OpenCVDemo.Steps;

public partial class AdaptiveThresholdViewModel : ObservableObject, IStep
{
    [ObservableProperty]
    string type = string.Empty;

    [ObservableProperty]
    string flag = string.Empty;

    [ObservableProperty]
    int blockSize = 7;

    [ObservableProperty]
    int c = 20;

    public ProcessResult Process(Mat image, string path, GenePool genes)
    {
        ThresholdTypes type = IStep.Parse<ThresholdTypes>(Type) ?? ThresholdTypes.Binary;
        AdaptiveThresholdTypes flag = IStep.Parse<AdaptiveThresholdTypes>(Flag) ?? AdaptiveThresholdTypes.GaussianC;

        int blockSize = genes[nameof(BlockSize)];
        if (blockSize % 2 == 0)
        {
            blockSize--;
        }

        int c = genes[nameof(C)].Current;
        Cv2.AdaptiveThreshold(image, image, 255, flag, type, blockSize, c);

        return new(image);
    }

    public int BlockSizeMinimum => 3;
    public int BlockSizeMaximum => 11;
    public int CMinimum => 0;
    public int CMaximum => 20;

    public GenePool Genes
    {
        get
        {
            return new GenePool(
                new Gene(nameof(BlockSize), BlockSizeMinimum, BlockSizeMaximum, BlockSize),
                new Gene(nameof(C), CMinimum, CMaximum, C));
        }

        set
        {
            BlockSize = value[nameof(BlockSize)];
            C = value[nameof(C)];
        }
    }
}
