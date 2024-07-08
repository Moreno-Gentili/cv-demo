using CommunityToolkit.Mvvm.ComponentModel;
using OpenCVDemo.Models;
using OpenCvSharp;

namespace OpenCVDemo.Steps;

public partial class CannyEdgeViewModel : ObservableObject, IStep
{
    [ObservableProperty]
    int threshold1 = 10;

    [ObservableProperty]
    int threshold2 = 40;

    public ProcessResult Process(Mat image, string path, GenePool genes)
    {
        int t1 = genes[nameof(Threshold1)];
        int t2 = genes[nameof(Threshold2)];
        image = image.Canny(Convert.ToDouble(t1), Convert.ToDouble(t2));
        return new(image);
    }

    public int Threshold1Minimum => 1;
    public int Threshold1Maximum => 800;
    public int Threshold2Minimum => 1;
    public int Threshold2Maximum => 800;

    public GenePool Genes
    {
        get
        {
            return new GenePool(
                new Gene(nameof(Threshold1), Threshold1Minimum, Threshold1Maximum, Threshold1),
                new Gene(nameof(Threshold2), Threshold2Minimum, Threshold2Maximum, Threshold2));
        }

        set
        {
            Threshold1 = value[nameof(Threshold1)];
            Threshold2 = value[nameof(Threshold2)];
        }
    }
}
