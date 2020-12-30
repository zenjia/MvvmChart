using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MvvmCharting.WpfFX;

namespace Demo
{
    public class SelectedSeriesShapeTypeToChartMinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SeriesMode t = (SeriesMode) value;
            switch (t)
            {
                case SeriesMode.Line:
                case SeriesMode.Area:
                    return DependencyProperty.UnsetValue;
                case SeriesMode.StackedArea:
                case SeriesMode.StackedArea100:
                    return 0.0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}