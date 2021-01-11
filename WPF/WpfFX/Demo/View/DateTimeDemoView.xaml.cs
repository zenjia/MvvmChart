using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MvvmCharting.Common;
using MvvmCharting;

namespace DemoViewModel
{

    public class DoubleToDateTimeStringConverter : IValueConverter
    {
        
 

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var q = (double)value;

            var t = DoubleValueConverter.DoubleToDateTimeOffset(q);

            return t.ToString("yyyy MMMM", new CultureInfo("en-US"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for DateTimeDemoView.xaml
    /// </summary>
    public partial class DateTimeDemoView : UserControl
    {
        public DateTimeDemoView()
        {
            InitializeComponent();
        }
    }
}
