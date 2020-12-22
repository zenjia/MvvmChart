using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{
    public class StepLineAreaGeometryBuilder : IGeometryBuilder
    {
        public Geometry GetGeometry(Point[] points)
        {
            Point[] arr = new Point[points.Length + 2 + (points.Length - 1)];
            arr[0] = new Point(points[0].X, 0);
            arr[arr.Length - 1] = new Point(points[points.Length - 1].X, 0);

            int j = 1;
            for (int i = 0; i < points.Length; i++)
            {
                arr[j++] = points[i];

                if (i < points.Length-1)
                {
                    arr[j++] = new Point(points[i + 1].X, points[i].Y);
                }
                
            }

            return PolyLineGeometryBuilder.CreateGeometry(arr, true);
        }
    }
}