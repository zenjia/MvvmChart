using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Demo
{
    

    public class IndexToStrokeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int index = (int) value;
            switch (index)
            {
                case 0:
                    return Brushes.CadetBlue;
                case 1:
                    return Brushes.Green;
                case 2:
                    return Brushes.Blue;
                default:
                    throw new ArgumentOutOfRangeException(index.ToString());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class IndexToScatterTemplateConverter : IValueConverter
    {
        public DataTemplate ScatterTemplate { get; set; }
        public DataTemplate Scatter2Template { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
 
            string s = (string) value;
            if (s == "Scatter2Template")
            {
                return this.Scatter2Template;
            }

            return this.ScatterTemplate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}