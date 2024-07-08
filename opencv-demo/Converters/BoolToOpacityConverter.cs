using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenCVDemo.Converters;

internal class BoolToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isEnabled && isEnabled)
        {
            return 1.0d;
        }
        else if (double.TryParse(parameter?.ToString(), CultureInfo.InvariantCulture, out double opacity))
        {
            return opacity;
        }
        else
        {
            return 0d;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
