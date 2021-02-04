using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MvvmCharting.Common;
#if NETCOREAPP
using Range = MvvmCharting.Common.Range;
#endif
namespace DemoViewModel
{
    public class RangeToPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Range range = (Range) value;

            return new Point(range.Min, range.Max);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for SeriesTemplateDemo.xaml
    /// </summary>
    public partial class GeneralDemoView : UserControl
    {
        public GeneralDemoView()
        {
            InitializeComponent();
        }

        
    }
}
