using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DemoViewModel
{
    public class ItemToScatterFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            SomePoint pt = (SomePoint)value;
            int i = (int)pt.t;


            if (i % 3 == 0)
            {
                return Brushes.Red;
            }


            return Brushes.LightSeaGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

     
    }
}