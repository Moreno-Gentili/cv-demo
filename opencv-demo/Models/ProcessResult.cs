using OpenCvSharp;
using Rect = System.Windows.Rect;

namespace OpenCVDemo.Models;

public record ProcessResult(Mat TransformedImage, Mat? Annotations = null, Action? SideEffects = null, Rect? Boundary = null);
