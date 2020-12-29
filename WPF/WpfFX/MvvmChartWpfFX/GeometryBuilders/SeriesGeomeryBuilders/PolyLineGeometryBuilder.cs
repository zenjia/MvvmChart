using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MvvmCharting.Drawing;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{
    public class PolyLineGeometryBuilder : ISeriesGeometryBuilder
    {
        public static Geometry CreateGeometry(PointNS[] points, PointNS[] previousPoints)
        {
 
          
            PointCollection pc = new PointCollection();
            foreach (var point in points.ToPoints())
            {
                pc.Add(point);
            }

            if (previousPoints != null)
            {
                foreach (var point in previousPoints.ToPointsReversed())
                {
                    pc.Add(point);
                }

                pc.Add(points[0].ToPoint());
            }



            PathFigure figure = new PathFigure();
            figure.StartPoint = points[0].ToPoint();
            PolyLineSegment polyLineSegment = new PolyLineSegment();
            polyLineSegment.IsStroked = true;
            polyLineSegment.Points = pc;
            
            figure.Segments.Add(polyLineSegment);
            if (previousPoints!=null)
            {
                figure.IsClosed = true;
                figure.IsFilled = true;
            }

            //figure.StartPoint = points[0].ToPoint();
            //var polyLineSegment = new PolyLineSegment(points.ToPoints(), true);
            //figure.Segments.Add(polyLineSegment);

            //if (previousPoints != null)
            //{
            //    var lineSegment = new LineSegment(previousPoints.Last().ToPoint(), false);
            //    figure.Segments.Add(lineSegment);

            //    //figure.StartPoint = previousPoints[0].ToPoint();
            //    polyLineSegment = new PolyLineSegment(previousPoints.ToPoints(), false);
            //    figure.Segments.Add(polyLineSegment);

            //    figure.StartPoint = previousPoints[0].ToPoint();
            //    lineSegment = new LineSegment(points.First().ToPoint(), false);
            //    figure.Segments.Add(lineSegment);

            //    figure.IsClosed = true;
            //    figure.IsFilled = true;
            //}


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



        public object GetGeometry(PointNS[] points, PointNS[] previousPoints)
        {
            return CreateGeometry(points, previousPoints);
        }
    }
}