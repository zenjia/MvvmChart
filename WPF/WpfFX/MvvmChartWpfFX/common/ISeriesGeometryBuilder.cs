using System.Windows;

namespace MvvmCharting.WpfFX
{
    public interface ISeriesGeometryBuilder
    {
        object GetGeometry(Point[] points, Point[] previousPoints);
    }
}