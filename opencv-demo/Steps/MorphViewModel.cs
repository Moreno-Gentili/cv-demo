using CommunityToolkit.Mvvm.ComponentModel;
using OpenCVDemo.Models;
using OpenCvSharp;

namespace OpenCVDemo.Steps;

public partial class MorphViewModel : ObservableObject, IStep
{
    [ObservableProperty]
    int size = 5;

    [ObservableProperty]
    string type = string.Empty;

    public ProcessResult Process(Mat image, string path, GenePool genes)
    {
        int size = genes[nameof(Size)].Current;
        Mat MorphKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(size, size));
        MorphTypes type = Parse<MorphTypes>(Type) ?? MorphTypes.Dilate;
        Cv2.MorphologyEx(image, image, type, MorphKernel);

        return new(image);
    }

    public int SizeMinimum => 1;
    public int SizeMaximum => 16;

    public GenePool Genes
    {
        get
        {
            return new GenePool(
                new Gene(nameof(Size), SizeMinimum, SizeMaximum, Size));
        }

        set
        {
            Size = Math.Max(value[nameof(Size)] / 2 * 2, value[nameof(Size)].Min);
        }
    }

    static T? Parse<T>(string value) where T : struct, Enum
    {
        if (Enum.TryParse(value.Trim('-'), ignoreCase: true, out T parsedValue))
        {
            return parsedValue;
        }

        return null;
    }
}
