using System.Windows;
using System.Windows.Media;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX.Series
{
    public class EllipseGeometryBuilder : IScatterGeometryBuilder
    {
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
 
        public object GetGeometry()
        {
            return new EllipseGeometry(new Point(RadiusX, RadiusY), RadiusX, RadiusY);
        }
    }
}