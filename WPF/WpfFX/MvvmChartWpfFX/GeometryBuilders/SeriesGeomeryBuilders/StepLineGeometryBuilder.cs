using System.Windows;
using MvvmCharting.Drawing;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{
    public class StepLineGeometryBuilder : ISeriesGeometryBuilder
    {
        public object GetGeometry(PointNS[] points, PointNS[] previousPoints)
        {
            PointNS[] arr;
            if (previousPoints!=null)
            {
                arr = new PointNS[points.Length + 2 + (points.Length - 1)];
                arr[0] = new PointNS(points[0].X, 0);
                arr[arr.Length - 1] = new PointNS(points[points.Length - 1].X, 0);

                int j = 1;
                for (int i = 0; i < points.Length; i++)
                {
                    arr[j++] = points[i];

                    if (i < points.Length - 1)
                    {
                        arr[j++] = new PointNS(points[i + 1].X, points[i].Y);
                    }

                }
            }
            else
            {
                arr = new PointNS[points.Length + (points.Length - 1)];
      
                int j = 0;
                for (int i = 0; i < points.Length; i++)
                {
                    arr[j++] = points[i];

                    if (i < points.Length - 1)
                    {
                        arr[j++] = new PointNS(points[i + 1].X, points[i].Y);
                    }

                }
            }
           

            return PolyLineGeometryBuilder.CreateGeometry(arr, previousPoints);
        }
    }
}