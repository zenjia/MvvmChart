using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Demo
{
    public class IndexToAreaSeriesFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int index = (int)value;
            switch (index)
            {
                case 1:
                    return Brushes.CadetBlue;
                case 2:
                    return Brushes.Green;
                case 0:
                    return Brushes.Blue;
                default:
                    return Brushes.Green;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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
                    return Brushes.YellowGreen;
                case 2:
                    return Brushes.Blue;
                default:
                    return Brushes.Red;  
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IndexToBarBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            switch (index)
            {
                case 0:
                    return Brushes.LightSeaGreen;
                case 1:
                    return Brushes.Crimson;
                case 2:
                    return Brushes.DarkSeaGreen;

            }

            return Brushes.OrangeRed;
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

            return ScatterDataTemplate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}