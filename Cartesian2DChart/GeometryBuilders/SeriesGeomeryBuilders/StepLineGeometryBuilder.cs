using System.Windows;
using System.Windows.Media;
using MvvmChart.Common.Drawing;

namespace MvvmCharting
{
    public class StepLineGeometryBuilder : ISeriesGeometryBuilder
    {
        
        public Geometry GetGeometry(PointNS[] points)
        {
            PointCollection pc = new PointCollection();
            foreach (var pt in points)
            {
                if (pc.Count > 0)
                {
                    pc.Add(new Point(pt.X, pc[pc.Count - 1].Y));
                }
                pc.Add(pt.ToPoint());
            }

            return PolyLineGeometryBuilder.CreateGeometry(pc);
 
        }
    }


}