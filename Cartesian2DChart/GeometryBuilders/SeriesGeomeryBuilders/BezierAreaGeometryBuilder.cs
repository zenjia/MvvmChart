using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{
    public class BezierAreaGeometryBuilder : ISeriesGeometryBuilder
    {
        public double Tension { get; set; } = 0.4;
        public Geometry GetGeometry(Point[] points)
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