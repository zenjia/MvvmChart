using MvvmCharting.Drawing;

namespace MvvmCharting.Series
{
    public interface ISeriesGeometryBuilder
    {
        object GetGeometry(PointNS[] points);
    }
}