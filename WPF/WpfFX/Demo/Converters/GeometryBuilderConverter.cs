using System;
using System.Globalization;
using System.Windows.Data;
using MvvmCharting.WpfFX.Series;

namespace DemoViewModel.WpfFX
{
    public class GeometryBuilderConverter : IValueConverter
    {
        public PolylineGeometryBuilder PolylineBuilder { get; set; }
        public StepLineGeometryBuilder StepLineBuilder { get; set; }
        public SplineGeometryBuilder SplineBuilder { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string t = (string) value;
            switch (t)
            {
                case "PolyLine":
                    return this.PolylineBuilder;
                case "StepLine":
                    return this.StepLineBuilder;
                case "Spline":
                    return this.SplineBuilder;

                default:
                    throw new ArgumentOutOfRangeException(t);

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}