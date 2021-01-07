using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MvvmCharting.Common; 

namespace MvvmCharting.WpfFX.Series
{
    /// <summary>
    /// Spline geometry builder.
    /// The spline generate algorithm is copy from
    /// https://docs.microsoft.com/en-us/archive/msdn-magazine/2010/january/extremeui-line-charts-with-data-templates
    /// Copyright@Petzold
    /// </summary>
    public class SplineGeometryBuilder : ISeriesGeometryBuilder
    {
        public double Tension { get; set; } = 0.5;
        public double Tolerance { get; set; } = 0.6;
        private static PointCollection CreateCurvePoints(Point[] points, 
            double tension = 0.5, double tolerance = 0.6)
        {
            if (points == null || points.Length < 1)
                return null;

            if (points.Length < 2)
                return null;

            PointCollection list = new PointCollection();
            if (points.Length == 2)
            {

                Segment(list, points[1], points[0], points[1], points[0], tension, tension, tolerance);
                Segment(list, points[0], points[1], points[0], points[1], tension, tension, tolerance);

            }
            else
            {
                for (int i = 0; i < points.Length; i++)
                {
                    double T1 = tension;
                    double T2 = tension;

                    if (i == 0)
                    {
                        Segment(list, points[0], points[0], points[1], points[2], T1, T2, tolerance);
                    }

                    else if (i == points.Length - 2)
                    {
                        Segment(list, points[i - 1], points[i], points[i + 1], points[i + 1], T1, T2, tolerance);
                    }

                    else if (i == points.Length - 1)
                    {

                    }
                    else
                    {
                        Segment(list, points[i - 1], points[i], points[i + 1], points[i + 2], T1, T2, tolerance);
                    }
                }
            }

            return list;
        }

        private static void Segment(PointCollection list, Point pt0, Point pt1, Point pt2, Point pt3, double T1, double T2, double tolerance)
        {
            // See Petzold, "Programming Microsoft Windows with C#", pages 645-646 or 
            //     Petzold, "Programming Microsoft Windows with Microsoft Visual Basic .NET", pages 638-639
            // for derivation of the following formulas:

            double SX1 = T1 * (pt2.X - pt0.X);
            double SY1 = T1 * (pt2.Y - pt0.Y);
            double SX2 = T2 * (pt3.X - pt1.X);
            double SY2 = T2 * (pt3.Y - pt1.Y);

            double AX = SX1 + SX2 + 2 * pt1.X - 2 * pt2.X;
            double AY = SY1 + SY2 + 2 * pt1.Y - 2 * pt2.Y;
            double BX = -2 * SX1 - SX2 - 3 * pt1.X + 3 * pt2.X;
            double BY = -2 * SY1 - SY2 - 3 * pt1.Y + 3 * pt2.Y;

            double CX = SX1;
            double CY = SY1;
            double DX = pt1.X;
            double DY = pt1.Y;

            int num = (int)((Math.Abs(pt1.X - pt2.X) + Math.Abs(pt1.Y - pt2.Y)) / tolerance);

            // Notice begins at 1 so excludes the first point (which is just pt1)
            for (int i = 1; i < num; i++)
            {
                double t = (double)i / (num - 1);
                Point pt = new Point(AX * t * t * t + BX * t * t + CX * t + DX,
                                     AY * t * t * t + BY * t * t + CY * t + DY);
                list.Add(pt);
            }
        }

        private static PathFigure CreateSpline(PointCollection points, PointCollection previousPoints)
        {
            PathFigure pathFigure = new PathFigure();

            pathFigure.StartPoint = points[0];


            var bezierSegment = new PolyLineSegment(points, true);
            pathFigure.Segments.Add(bezierSegment);

            if (previousPoints != null)
            {
                var lineSegment = new LineSegment(previousPoints.First(), false);
                pathFigure.Segments.Add(lineSegment);

                if (previousPoints.Count > 2)
                {

                    bezierSegment = new PolyLineSegment(previousPoints, false);
                    pathFigure.Segments.Add(bezierSegment);
                }
                else
                {
                    var polyLineSegment = new PolyLineSegment(previousPoints, false);
                    pathFigure.Segments.Add(polyLineSegment);
                }

                lineSegment = new LineSegment(points[0], false);
                pathFigure.Segments.Add(lineSegment);

                pathFigure.IsClosed = true;
                pathFigure.IsFilled = true;
            }

            return pathFigure;
        }

        public object GetGeometry(Point[] points, Point[] previousPoints)
        {
            if (points.Length < 2)
            {
                return PolylineGeometryBuilder.CreateGeometry(points, previousPoints);
            }

            var curvePoints = CreateCurvePoints(points, this.Tension);
            PointCollection curvePoints2 = null;
            if (previousPoints != null)
            {
                if (previousPoints.Length < 2)
                {
                    throw new MvvmChartUnexpectedTypeException($"GetGeometry: previousPoints.Length={previousPoints.Length < 2}");
                }

                if (previousPoints.Length == 2)
                {
                    curvePoints2 = new PointCollection();
                    for (int i = 0; i < previousPoints.Length; i++)
                    {
                        curvePoints2.Add(previousPoints[previousPoints.Length - 1 - i]);
                    }
                }
                else
                {

                    curvePoints2 = CreateCurvePoints(previousPoints.Reversed(), this.Tension);
                }
            }

            var figure = CreateSpline(curvePoints, curvePoints2);
            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(figure);
            figure.IsClosed = previousPoints != null;
            figure.Freeze();

            return pathGeometry;
        }

    }
}