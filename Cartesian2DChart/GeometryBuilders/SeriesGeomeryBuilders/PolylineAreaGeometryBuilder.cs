using System;
using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{
    public class PolylineAreaGeometryBuilder : ISeriesGeometryBuilder
    {
        public static Geometry CreateGeometry(Point[] points)
        {
            Point[] arr = new Point[points.Length + 2];
            arr[0] = new Point(points[0].X, 0);
            arr[arr.Length - 1] = new Point(points[points.Length - 1].X, 0);
            for (int i = 0; i < points.Length; i++)
            {
                arr[i + 1] = points[i];
            }
            return PolyLineGeometryBuilder.CreateGeometry(arr, true);
        }

        public Geometry GetGeometry(Point[] points)
        {
            return CreateGeometry(points);
        }
    }
}