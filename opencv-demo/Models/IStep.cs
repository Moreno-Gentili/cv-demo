using OpenCvSharp;
using System.ComponentModel;

namespace OpenCVDemo.Models;

public interface IStep : INotifyPropertyChanged
{
    ProcessResult Process(Mat image, string path) => Process(image, path, Genes);
    ProcessResult Process(Mat image, string path, GenePool genes);
    GenePool Genes { get; set; }

    static T? Parse<T>(string value) where T : struct, Enum
    {
        if (Enum.TryParse(value.Trim('-'), ignoreCase: true, out T parsedValue))
        {
            return parsedValue;
        }

        return null;
    }
}
