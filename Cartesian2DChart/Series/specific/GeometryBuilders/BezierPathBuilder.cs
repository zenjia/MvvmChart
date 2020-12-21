using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{

    /// <summary>
    /// Source: http://csharphelper.com/blog/2019/04/draw-a-smooth-curve-in-wpf-and-c/
    /// </summary>
    public class BezierGeometryBuilder : IGeometryBuilder
    {
        // Make a Path holding a series of Bezier curves.
        // The points parameter includes the points to visit
        // and the control points.
        private PathGeometry MakeBezierPath(Point[] points)
        {
            // Add a PathGeometry.
            PathGeometry path_geometry = new PathGeometry();

            
            // Create a PathFigure.
            PathFigure path_figure = new PathFigure();
            path_geometry.Figures.Add(path_figure);

            // Start at the first point.
            path_figure.StartPoint = points[0];

            // Create a PathSegmentCollection.
            PathSegmentCollection path_segment_collection =
                new PathSegmentCollection();
            path_figure.Segments = path_segment_collection;

            // Add the rest of the points to a PointCollection.
            PointCollection point_collection =
                new PointCollection(points.Length - 1);
            for (int i = 1; i < points.Length; i++)
                point_collection.Add(points[i]);

            // Make a PolyBezierSegment from the points.
            PolyBezierSegment bezier_segment = new PolyBezierSegment();
            bezier_segment.Points = point_collection;
            
            // Add the PolyBezierSegment to othe segment collection.
            path_segment_collection.Add(bezier_segment);

            path_figure.Freeze();

            return path_geometry;
        }

        // Make an array containing Bezier curve points and control points.
        private Point[] MakeCurvePoints(Point[] points, double tension)
        {
            if (points.Length < 2)
            {
                return null;
            }

            double control_scale = tension / 0.5 * 0.175;

            // Make a list containing the points and
            // appropriate control points.
            List<Point> result_points = new List<Point>();
            result_points.Add(points[0]);

            for (int i = 0; i < points.Length - 1; i++)
            {
                // Get the point and its neighbors.
                Point pt_before = points[Math.Max(i - 1, 0)];
                Point pt = points[i];
                Point pt_after = points[i + 1];
                Point pt_after2 = points[Math.Min(i + 2, points.Length - 1)];

                double dx1 = pt_after.X - pt_before.X;
                double dy1 = pt_after.Y - pt_before.Y;

                Point p1 = points[i];
                Point p4 = pt_after;

                double dx = pt_after.X - pt_before.X;
                double dy = pt_after.Y - pt_before.Y;
                Point p2 = new Point(
                    pt.X + control_scale * dx,
                    pt.Y + control_scale * dy);

                dx = pt_after2.X - pt.X;
                dy = pt_after2.Y - pt.Y;
                Point p3 = new Point(
                    pt_after.X - control_scale * dx,
                    pt_after.Y - control_scale * dy);

                // Save points p2, p3, and p4.
                result_points.Add(p2);
                result_points.Add(p3);
                result_points.Add(p4);
            }

            // Return the points.
            return result_points.ToArray();
        }

        public double Tension { get; set; } = 0.4;

        // Make a Bezier curve connecting these points.
        public Geometry GetGeometry(Point[] points)
        {
            if (points.Length < 2)
            {
                return PolyLineGeometryBuilder.CreateGeometry(points);
            }

            Point[] result_points = MakeCurvePoints(points, this.Tension);

            // Use the points to create the path.
            return MakeBezierPath(result_points.ToArray());
        }

 
    }
}