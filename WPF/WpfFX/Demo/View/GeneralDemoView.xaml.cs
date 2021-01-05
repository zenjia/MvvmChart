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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo
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
