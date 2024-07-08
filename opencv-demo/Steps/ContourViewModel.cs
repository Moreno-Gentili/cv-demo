using CommunityToolkit.Mvvm.ComponentModel;
using OpenCVDemo.Models;
using OpenCvSharp;
using WRect = System.Windows.Rect;

namespace OpenCVDemo.Steps;

public partial class ContourViewModel : ObservableObject, IStep
{
    [ObservableProperty]
    string mode = string.Empty;

    [ObservableProperty]
    int area;

    public ProcessResult Process(Mat image, string path, GenePool genes)
    {
        RetrievalModes mode = RetrievalModes.List;

        ContourApproximationModes approximation = ContourApproximationModes.ApproxSimple;
        Point[][] contours = image.FindContoursAsArray(mode, approximation);

        OnEdgeMode edgeMode = IStep.Parse<OnEdgeMode>(Mode) ?? OnEdgeMode.Keep;
        List<Point[]> filteredContours = FilterContours(contours, image, genes[nameof(Area)], edgeMode).ToList();

        Mat annotations = new(image.Size(), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
        Scalar opaqueRed = new Scalar(0, 0, 255, 255);
        Scalar opaqueGreen = new Scalar(0, 128, 0, 255);
        Cv2.DrawContours(annotations, filteredContours, -1, opaqueRed, Convert.ToInt32(Math.Ceiling(image.Width / 400.0)));

        WRect? wRect = null;
        if (filteredContours.Count > 0)
        {
            Rect boundingBox = filteredContours.Select(Cv2.BoundingRect).Aggregate((a, b) => a.Union(b));
            Cv2.Rectangle(annotations, boundingBox, opaqueGreen, Convert.ToInt32(Math.Ceiling(image.Width / 200d)));
            wRect = new(boundingBox.Left, boundingBox.Top, boundingBox.Width, boundingBox.Height);
        }

        return new(image, annotations, null, wRect);
    }

    public int AreaMinimum => 0;
    public int AreaMaximum => 20;

    public GenePool Genes
    {
        get
        {
            return new GenePool(
                new Gene(nameof(Area), AreaMinimum, AreaMaximum, Area));
        }

        set
        {
            Area = value[nameof(Area)];
        }
    }

    private enum OnEdgeMode
    {
        Remove,
        Keep
    }

    static IEnumerable<Point[]> FilterContours(Point[][] contours, Mat image, int minimumArea, OnEdgeMode edgeMode)
    {
        foreach (var contour in contours)
        {
            double actualArea = Cv2.ContourArea(contour);
            if (actualArea >= minimumArea)
            {
                bool canAdd = true;
                if (edgeMode != OnEdgeMode.Keep)
                {
                    canAdd = contour.All(point =>
                    {
                        bool IsXInside = point.X > 0 && point.X < image.Width - 1;
                        bool IsYInside = point.Y > 0 && point.Y < image.Height - 1;
                        return IsXInside && IsYInside;
                    });
                }

                if (canAdd)
                {
                    yield return contour;
                }
            }
        }
    }
}
