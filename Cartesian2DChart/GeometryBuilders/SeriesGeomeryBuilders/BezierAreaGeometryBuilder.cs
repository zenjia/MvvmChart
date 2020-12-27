using System.Windows.Media;
using MvvmCharting.Drawing;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{
    public class BezierAreaGeometryBuilder : ISeriesGeometryBuilder
    {
        public double Tension { get; set; } = 0.4;
        public object GetGeometry(PointNS[] points)
        {
            if (points.Length < 2)
            {
                return PolylineAreaGeometryBuilder.CreateGeometry(points);
            }

            var curvePoints = BezierGeometryBuilder.MakeCurvePoints(points, this.Tension);
            var figure = BezierGeometryBuilder.MakeBezierPathFigure(curvePoints, true);
            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(figure);
            figure.IsClosed = true;
            figure.Freeze();

            return pathGeometry;
        }
    }
}