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
using MvvmCharting.Series;
using MvvmCharting.WpfFX;

namespace Demo.WpfFX
{
    public class GeometryBuilderConverter : IValueConverter
    {
        public PolyLineGeometryBuilder PolyLineBuilder { get; set; }
        public StepLineGeometryBuilder StepLineBuilder { get; set; }
        public SplineGeometryBuilder SplineBuilder { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string t = (string) value;
            switch (t)
            {
                case "PolyLine":
                    return PolyLineBuilder;
                case "StepLine":
                    return StepLineBuilder;
                case "Spline":
                    return SplineBuilder;

                default:
                    throw new ArgumentOutOfRangeException(t);

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for SeriesCustomizingDemoView.xaml
    /// </summary>
    public partial class SeriesCustomizingDemoView : UserControl
    {
        public SeriesCustomizingDemoView()
        {
            InitializeComponent();
        }
    }
}
