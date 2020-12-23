using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{
    public class EllipseGeometryBuilder : IScatterGeometryBuilder
    {
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
 
        public Geometry GetGeometry()
        {
            return new EllipseGeometry(new Point(RadiusX, RadiusY), RadiusX, RadiusY);
        }
    }
}