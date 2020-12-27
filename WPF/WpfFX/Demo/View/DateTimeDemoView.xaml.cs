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

namespace Demo
{

    public class DoubleToDateTimeStringConverter : IValueConverterNS
    {
 

        public object ConverterTo(object value, CultureInfo culture)
        {
            var q = (double)value;



            var t = DoubleValueConverter.DoubleToDateTimeOffset(q);

            return t.ToString("yyyy MMMM dd", new CultureInfo("en-US"));
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
