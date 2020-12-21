using System;
using System.Globalization;
using System.Windows.Data;

namespace MvvmCharting.Axis
{
    internal class DefaultAxisValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format("{0:F1}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}