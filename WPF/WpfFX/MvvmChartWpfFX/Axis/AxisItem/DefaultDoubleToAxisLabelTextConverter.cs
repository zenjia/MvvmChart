using System;
using System.Globalization;
using System.Windows.Data;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// Default double to string converter for AxisLabelText.
    /// </summary>
    public class DefaultDoubleToAxisLabelTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{value:F2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}