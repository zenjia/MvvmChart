using System.Windows;
using System.Windows.Media;
using MvvmCharting.Drawing;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{
    public class PolyLineGeometryBuilder : ISeriesGeometryBuilder
    {
        internal static Geometry CreateGeometry(PointNS[] points, bool isClosed = false)
        {
            PathGeometry path_geometry = new PathGeometry();

            PolyLineSegment polyLineSegment = new PolyLineSegment();
            var pointCollection = new PointCollection();
            for (int i = 0; i < points.Length; i++)
            {
                pointCollection.Add(new Point(points[i].X, points[i].Y));
            }

            polyLineSegment.Points = pointCollection;


            PathFigure figure = new PathFigure();
            figure.Segments.Add(polyLineSegment);
            if (isClosed)
            {
                figure.IsClosed = true;
                figure.IsFilled = true;
            }

            path_geometry.Figures.Add(figure);

            path_geometry.Freeze();
            return path_geometry;
        }

        internal static Geometry CreateGeometry(PointCollection pointCollection)
        {
            var path_geometry = new PathGeometry();

            var polyLineSegment = new PolyLineSegment();
            polyLineSegment.Points = pointCollection;

            var figure = new PathFigure();
            figure.Segments.Add(polyLineSegment);

            path_geometry.Figures.Add(figure);

            path_geometry.Freeze();
            return path_geometry;
        }

        public object GetGeometry(PointNS[] points)
        {
            return CreateGeometry(points);

        }
    }
}