using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Demo
{

    public class StringToScatterTemplateConverter : IValueConverter
    {
        public DataTemplate ScatterTemplate { get; set; }
        public DataTemplate Scatter2Template { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "ScatterTemplate")
            {
                return this.ScatterTemplate;
            }

            return this.Scatter2Template;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for BigDataTestView.xaml
    /// </summary>
    public partial class BigDataTestView : UserControl
    {
        public BigDataTestView()
        {
            InitializeComponent();
        }
    }
}
