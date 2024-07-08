using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace OpenAIDemo.Converters;

public class DoubleToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((double)value).ToString("0.0");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (double.TryParse(value as string, out double result))
        {
            return result;
        }
        return DependencyProperty.UnsetValue;
    }
}
