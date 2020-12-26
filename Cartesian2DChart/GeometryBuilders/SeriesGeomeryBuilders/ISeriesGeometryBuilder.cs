using System.Windows;
using System.Windows.Media;
using MvvmChart.Common.Drawing;

namespace MvvmCharting
{
    public interface ISeriesGeometryBuilder
    {
        Geometry GetGeometry(PointNS[] points);
    }
}