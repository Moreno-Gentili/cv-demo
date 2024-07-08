using CommunityToolkit.Mvvm.ComponentModel;
using OpenCVDemo.Models;
using OpenCvSharp;

namespace OpenCVDemo.Steps;

public partial class GrayscaleViewModel : ObservableObject, IStep
{
    public ProcessResult Process(Mat image, string path, GenePool genes)
    {
        Mat output = image;
        Cv2.CvtColor(image, output, ColorConversionCodes.RGB2GRAY);
        return new(output);
    }

    public GenePool Genes
    {
        get
        {
            return new GenePool();
        }

        set
        {
        }
    }
}
