using System.Windows;
using MvvmCharting.Drawing;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{
    public class StepLineGeometryBuilder : ISeriesGeometryBuilder
    {
        private static PointNS[] ConvertToStepPoints(PointNS[] points)
        {
            if (points == null)
            {
                return null;
            }
            var arr = new PointNS[points.Length + (points.Length - 1)];

            int j = 0;
            for (int i = 0; i < points.Length; i++)
            {
                arr[j++] = points[i];

                if (i < points.Length - 1)
                {
                    arr[j++] = new PointNS(points[i + 1].X, points[i].Y);
                }

            }

            return arr;
        }

        public object GetGeometry(PointNS[] points, PointNS[] previousPoints)
        {
            var arr = ConvertToStepPoints(points);
            var arr2 = ConvertToStepPoints(previousPoints);

            return PolyLineGeometryBuilder.CreateGeometry(arr, arr2);
        }
    }
}