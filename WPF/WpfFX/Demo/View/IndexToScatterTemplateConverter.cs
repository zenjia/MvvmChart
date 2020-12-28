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
        public DataTemplate ScatterDataTemplate { get; set; }
        public DataTemplate ScatterDataTemplate1 { get; set; }
        public DataTemplate ScatterDataTemplate2 { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int index = (int) value;
            switch (index)
            {
                case 0:
                    return this.ScatterDataTemplate;
                case 1:
                    return this.ScatterDataTemplate1;
                case 2:
                    return this.ScatterDataTemplate2;
                
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}