using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Demo
{
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