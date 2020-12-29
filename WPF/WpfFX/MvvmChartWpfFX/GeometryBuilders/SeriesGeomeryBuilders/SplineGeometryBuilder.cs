using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MvvmCharting.Common;
using MvvmCharting.Drawing;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{



    /// <summary>
    /// Source: http://csharphelper.com/blog/2019/04/draw-a-smooth-curve-in-wpf-and-c/
    /// </summary>
    public class SplineGeometryBuilder : ISeriesGeometryBuilder
    {
        // Make an array containing Bezier curve points and control points.
        public static Point[] MakeCurvePoints(PointNS[] points, double tension)
        {
            if (points == null || points.Length < 2)
            {
                return null;
            }

            double control_scale = tension / 0.5 * 0.175;

            // Make a list containing the points and
            // appropriate control points.
            List<Point> result_points = new List<Point>();
            result_points.Add(points[0].ToPoint());

            for (int i = 0; i < points.Length - 1; i++)
            {
                // Get the point and its neighbors.
                Point pt_before = points[Math.Max(i - 1, 0)].ToPoint();
                Point pt = points[i].ToPoint();
                Point pt_after = points[i + 1].ToPoint();
                Point pt_after2 = points[Math.Min(i + 2, points.Length - 1)].ToPoint();

                double dx1 = pt_after.X - pt_before.X;
                double dy1 = pt_after.Y - pt_before.Y;

                Point p1 = points[i].ToPoint();
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

        public static PathFigure MakeBezierPathFigure(Point[] points, Point[] previousPoints)
        {
            PathFigure pathFigure = new PathFigure();
             
            pathFigure.StartPoint = points[0];

            var bezierSegment = new PolyBezierSegment(points, true);
            pathFigure.Segments.Add(bezierSegment);

            if (previousPoints != null)
            {
                var lineSegment = new LineSegment(previousPoints.Last(), false);
                pathFigure.Segments.Add(lineSegment);

                if (previousPoints.Length > 2)
                {
 
                    bezierSegment = new PolyBezierSegment(previousPoints.Reversed(), false);
                    pathFigure.Segments.Add(bezierSegment);
                }
                else
                {
                    var polyLineSegment = new PolyLineSegment(previousPoints.Reversed(), false);
                    pathFigure.Segments.Add(polyLineSegment);
                }

                lineSegment = new LineSegment(points[0], false);
                pathFigure.Segments.Add(lineSegment);

                pathFigure.IsClosed = true;
                pathFigure.IsFilled = true;
            }

            return pathFigure;
        }


        public double Tension { get; set; } = 0.4;
        public object GetGeometry(PointNS[] points, PointNS[] previousPoints)
        {
            if (points.Length < 2)
            {
                return PolyLineGeometryBuilder.CreateGeometry(points, previousPoints);
            }

            var curvePoints = MakeCurvePoints(points, this.Tension);
            Point[] curvePoints2 = null;
            if (previousPoints != null)
            {
                if (previousPoints.Length < 2)
                {
                    throw new MvvmChartUnexpectedTypeException($"GetGeometry: previousPoints.Length={previousPoints.Length < 2}");
                }

                if (previousPoints.Length == 2) 
                {
                    curvePoints2 = new Point[previousPoints.Length];
                    for (int i = 0; i < curvePoints2.Length; i++)
                    {
                        curvePoints2[i] = previousPoints[i].ToPoint();
                    }
                }
                else
                {
 
                    curvePoints2 = MakeCurvePoints(previousPoints, this.Tension);
                }
            }

            var figure = MakeBezierPathFigure(curvePoints, curvePoints2);
            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(figure);
            figure.IsClosed = previousPoints != null;
            figure.Freeze();

            return pathGeometry;
        }
    }
}