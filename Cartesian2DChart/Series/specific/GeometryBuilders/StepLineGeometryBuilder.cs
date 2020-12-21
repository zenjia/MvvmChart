using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{
    public class StepLineGeometryBuilder : IGeometryBuilder
    {
        public Geometry GetGeometry(Point[] points)
        {
            PointCollection pc = new PointCollection();
            foreach (var pt in points)
            {
                if (pc.Count > 0)
                {
                    pc.Add(new Point(pt.X, pc[pc.Count - 1].Y));
                }
                pc.Add(pt);
            }

            return PolyLineGeometryBuilder.CreateGeometry(pc);
 
        }
    }
}