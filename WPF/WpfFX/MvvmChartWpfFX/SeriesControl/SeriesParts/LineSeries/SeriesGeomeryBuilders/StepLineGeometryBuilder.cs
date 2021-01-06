 

using System.Windows;

namespace MvvmCharting.WpfFX.Series
{
    public class StepLineGeometryBuilder : ISeriesGeometryBuilder
    {
        private static Point[] ConvertToStepPoints(Point[] points)
        {
            if (points == null)
            {
                return null;
            }
            var arr = new Point[points.Length + (points.Length - 1)];

            int j = 0;
            for (int i = 0; i < points.Length; i++)
            {
                arr[j++] = points[i];

                if (i < points.Length - 1)
                {
                    arr[j++] = new Point(points[i + 1].X, points[i].Y);
                }

            }

            return arr;
        }

        public object GetGeometry(Point[] points, Point[] previousPoints)
        {
            var arr = ConvertToStepPoints(points);
            var arr2 = ConvertToStepPoints(previousPoints);

            return PolylineGeometryBuilder.CreateGeometry(arr, arr2);
        }
    }
}