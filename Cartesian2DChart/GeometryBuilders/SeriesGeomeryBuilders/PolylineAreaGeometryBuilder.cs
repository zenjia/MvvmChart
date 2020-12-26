using System.Windows;
using System.Windows.Media;
using MvvmChart.Common.Drawing;

namespace MvvmCharting
{
    public class PolylineAreaGeometryBuilder : ISeriesGeometryBuilder
    {
        public static Geometry CreateGeometry(PointNS[] points)
        {
            PointNS[] arr = new PointNS[points.Length + 2];
            arr[0] = new PointNS(points[0].X, 0);
            arr[arr.Length - 1] = new PointNS(points[points.Length - 1].X, 0);
            for (int i = 0; i < points.Length; i++)
            {
                arr[i + 1] = points[i];
            }
            return PolyLineGeometryBuilder.CreateGeometry(arr, true);
        }

        public Geometry GetGeometry(PointNS[] points)
        {
            return CreateGeometry(points);
        }
    }
}