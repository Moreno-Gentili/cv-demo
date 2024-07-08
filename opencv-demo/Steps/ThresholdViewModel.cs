using CommunityToolkit.Mvvm.ComponentModel;
using OpenCVDemo.Models;
using OpenCvSharp;
using System.Text;
using System.Windows.Media;

namespace OpenCVDemo.Steps;

public partial class ThresholdViewModel : ObservableObject, IStep
{
    string? path;
   
    [ObservableProperty]
    Geometry? histogram;

    [ObservableProperty]
    string thresholdType = string.Empty;

    [ObservableProperty]
    string thresholdFlag = string.Empty;

    [ObservableProperty]
    double threshold = 90;

    public ProcessResult Process(Mat image, string path, GenePool genes)
    {
        ThresholdTypes type = GetThresholdType();
        Geometry? histogram = GetHistogramIfNeeded(image, path);

        double threshold = Cv2.Threshold(image, image, Threshold, 255, type);
        
        Action sideEffects = () =>
        {
            if (type.HasFlag(ThresholdTypes.Otsu) || type.HasFlag(ThresholdTypes.Triangle))
            {
                Threshold = threshold;
            }

            if (histogram is not null)
            {
                Histogram = histogram;
            }
        };

        return new(image, SideEffects: sideEffects);
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

    ThresholdTypes GetThresholdType()
    {
        ThresholdTypes type = IStep.Parse<ThresholdTypes>(ThresholdType) ?? ThresholdTypes.Binary;
        ThresholdTypes? flag = IStep.Parse<ThresholdTypes>(ThresholdFlag);
        if (flag is not null)
        {
            type |= flag.Value;
        }

        return type;
    }

    Geometry? GetHistogramIfNeeded(Mat image, string path)
    {
        if (this.path != path || Histogram is null)
        {
            this.path = path;
            return CalculateHistogram(image);
        }
        else
        {
            return null;
        }
    }

    static Geometry CalculateHistogram(Mat input)
    {
        Mat hist = new(256, 1, MatType.CV_32FC1);
        Cv2.CalcHist([input], [0], null, hist, 1, [hist.Rows], [[0, 256]]);
        return Plot(hist);
    }
    static Geometry Plot(Mat hist)
    {
        StringBuilder sb = new();
        var start = hist.At<float>(0, 0);
        hist.GetArray(out float[] data);
        float max = data.Max();
        var ints = data.Select(d => Convert.ToInt32((1.0 - (d / max)) * 80)).ToArray();
        sb.Append($"M 0,{ints[0]}");

        for (int i = 0; i < ints.Length; i++)
        {
            sb.Append($" L{i},{ints[i]}");
        }

        return Geometry.Parse(sb.ToString());
    }
}
