using CommunityToolkit.Mvvm.ComponentModel;
using OpenCVDemo.Models;
using OpenCvSharp;

namespace OpenCVDemo.Steps;

public partial class SubtractViewModel : ObservableObject, IStep
{
    public ProcessResult Process(Mat image, string path, GenePool genes)
    {
        /*
         *  Mat mask = new Mat(img.Width, img.Height, img.Type());
            Cv2.InRange(img, new Scalar(0), new Scalar(SubtractThreshold.Value), mask);
            Cv2.Subtract(img, new Scalar(SubtractAmount.Value), img, mask);
            mask.Dispose();
        */
        return new(image);
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
