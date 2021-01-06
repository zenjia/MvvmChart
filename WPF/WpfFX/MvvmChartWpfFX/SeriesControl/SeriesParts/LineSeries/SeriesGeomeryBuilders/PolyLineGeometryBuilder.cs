using System.Linq;
using System.Windows;
using System.Windows.Media; 

namespace MvvmCharting.WpfFX.Series
{
    public class PolyLineGeometryBuilder : ISeriesGeometryBuilder
    {
        public static Geometry CreateGeometry(Point[] points, Point[] previousPoints)
        {
 
          
            //PointCollection pc = new PointCollection();
            //foreach (var point in points.ToPoints())
            //{
            //    pc.Add(point);
            //}

            //if (previousPoints != null)
            //{
            //    foreach (var point in previousPoints.ToPointsReversed())
            //    {
            //        pc.Add(point);
            //    }

            //    pc.Add(points[0].ToPoint());
            //}



            //PathFigure figure = new PathFigure();
            //figure.StartPoint = points[0].ToPoint();
            //PolyLineSegment polyLineSegment = new PolyLineSegment();
            //polyLineSegment.IsStroked = true;
            //polyLineSegment.Points = pc;
            
            //figure.Segments.Add(polyLineSegment);
            //if (previousPoints!=null)
            //{
            //    figure.IsClosed = true;
            //    figure.IsFilled = true;
            //}

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

        //internal static Geometry CreateGeometry(PointCollection pointCollection)
        //{
        //    var path_geometry = new PathGeometry();

        //    var polyLineSegment = new PolyLineSegment();
        //    polyLineSegment.Points = pointCollection;

        //    var figure = new PathFigure();
        //    figure.Segments.Add(polyLineSegment);

        //    path_geometry.Figures.Add(figure);

        //    path_geometry.Freeze();
        //    return path_geometry;
        //}



        public object GetGeometry(Point[] points, Point[] previousPoints)
        {
            return CreateGeometry(points, previousPoints);
        }
    }
}