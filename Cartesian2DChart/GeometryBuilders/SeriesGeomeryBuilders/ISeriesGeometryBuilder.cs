using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{
    public interface ISeriesGeometryBuilder
    {
        Geometry GetGeometry(Point[] points);
    }
}