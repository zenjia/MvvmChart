using System.Linq;
using System.Windows;
using System.Windows.Media; 

namespace MvvmCharting.WpfFX.Series
{
    public class PolylineGeometryBuilder : ISeriesGeometryBuilder
    {
        public static Geometry CreateGeometry(Point[] points, Point[] previousPoints)
        {

            PathFigure figure = new PathFigure();
            figure.StartPoint = points[0];
            var polyLineSegment = new PolyLineSegment(points, true);
            figure.Segments.Add(polyLineSegment);

            if (previousPoints != null)
            {
                var lineSegment = new LineSegment(previousPoints.Last(), false);
                figure.Segments.Add(lineSegment);

                //figure.StartPoint = previousPoints[0].ToPoint();
                polyLineSegment = new PolyLineSegment(previousPoints.Reversed(), false);
                figure.Segments.Add(polyLineSegment);

                lineSegment = new LineSegment(points.First(), false);
                figure.Segments.Add(lineSegment);

                figure.IsClosed = true;
                figure.IsFilled = true;
            }


            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(figure);

            pathGeometry.Freeze();
            return pathGeometry;
        }

 


        public object GetGeometry(Point[] points, Point[] previousPoints)
        {
            return CreateGeometry(points, previousPoints);
        }
    }
}