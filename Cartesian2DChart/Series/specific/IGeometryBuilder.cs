using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{
    public interface IGeometryBuilder
    {
        Geometry GetGeometry(Point[] points);
    }
}