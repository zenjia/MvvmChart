using System.Windows;
using System.Windows.Media;
using MvvmChart.Common.Drawing;

namespace MvvmCharting
{
    public class StepLineAreaGeometryBuilder : ISeriesGeometryBuilder
    {
        public Geometry GetGeometry(PointNS[] points)
        {
            PointNS[] arr = new PointNS[points.Length + 2 + (points.Length - 1)];
            arr[0] = new PointNS(points[0].X, 0);
            arr[arr.Length - 1] = new PointNS(points[points.Length - 1].X, 0);

            int j = 1;
            for (int i = 0; i < points.Length; i++)
            {
                arr[j++] = points[i];

                if (i < points.Length-1)
                {
                    arr[j++] = new PointNS(points[i + 1].X, points[i].Y);
                }
                
            }

            return PolyLineGeometryBuilder.CreateGeometry(arr, true);
        }
    }
}