using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MvvmCharting
{
    public class CanonicalSpline : Shape
    {
        // Cached PathGeometry
        PathGeometry pathGeometry;

        // Dependency Properties
        public static readonly DependencyProperty PointsProperty =
            Polyline.PointsProperty.AddOwner(typeof(CanonicalSpline),
                new FrameworkPropertyMetadata(null, OnMeasurePropertyChanged));

        public static readonly DependencyProperty TensionProperty =
            DependencyProperty.Register("Tension",
                typeof(double),
                typeof(CanonicalSpline),
                new FrameworkPropertyMetadata(0.3, OnMeasurePropertyChanged));

        public static readonly DependencyProperty TensionsProperty =
            DependencyProperty.Register("Tensions",
                typeof(DoubleCollection),
                typeof(CanonicalSpline),
                new FrameworkPropertyMetadata(null, OnMeasurePropertyChanged));

        public static readonly DependencyProperty IsClosedProperty =
            PathFigure.IsClosedProperty.AddOwner(typeof(CanonicalSpline),
                new FrameworkPropertyMetadata(false, OnMeasurePropertyChanged));

        public static readonly DependencyProperty IsFilledProperty =
            PathFigure.IsFilledProperty.AddOwner(typeof(CanonicalSpline),
                new FrameworkPropertyMetadata(false, OnMeasurePropertyChanged));

        public static readonly DependencyProperty FillRuleProperty =
            Polyline.FillRuleProperty.AddOwner(typeof(CanonicalSpline),
                new FrameworkPropertyMetadata(FillRule.EvenOdd, OnRenderPropertyChanged));

        public static readonly DependencyProperty ToleranceProperty =
            DependencyProperty.Register("Tolerance",
                typeof(double),
                typeof(CanonicalSpline),
                new FrameworkPropertyMetadata(Geometry.StandardFlatteningTolerance, OnMeasurePropertyChanged));

        // CLR properties
        public PointCollection Points
        {
            set { SetValue(PointsProperty, value); }
            get { return (PointCollection)GetValue(PointsProperty); }
        }

        public double Tension
        {
            set { SetValue(TensionProperty, value); }
            get { return (double)GetValue(TensionProperty); }
        }

        public DoubleCollection Tensions
        {
            set { SetValue(TensionsProperty, value); }
            get { return (DoubleCollection)GetValue(TensionsProperty); }
        }

        public bool IsClosed
        {
            set { SetValue(IsClosedProperty, value); }
            get { return (bool)GetValue(IsClosedProperty); }
        }

        public bool IsFilled
        {
            set { SetValue(IsFilledProperty, value); }
            get { return (bool)GetValue(IsFilledProperty); }
        }

        public FillRule FillRule
        {
            set { SetValue(FillRuleProperty, value); }
            get { return (FillRule)GetValue(FillRuleProperty); }
        }

        public double Tolerance
        {
            set { SetValue(ToleranceProperty, value); }
            get { return (double)GetValue(ToleranceProperty); }
        }

        // Required DefiningGeometry override
        protected override Geometry DefiningGeometry
        {
            get { return this.pathGeometry; }
        }

        // Property-Changed handlers
        static void OnMeasurePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as CanonicalSpline).OnMeasurePropertyChanged(args);
        }

        void OnMeasurePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            this.pathGeometry = CreateSpline(this.Points, this.Tension, this.Tensions, this.IsClosed, this.IsFilled, this.Tolerance);
            InvalidateMeasure();
            OnRenderPropertyChanged(args);
        }

        static void OnRenderPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as CanonicalSpline).OnRenderPropertyChanged(args);
        }

        void OnRenderPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.pathGeometry != null)
                this.pathGeometry.FillRule = this.FillRule;

            InvalidateVisual();
        }

        static PathGeometry CreateSpline(PointCollection pts, double tension, DoubleCollection tensions,
            bool isClosed, bool isFilled, double tolerance)
        {
            if (pts == null || pts.Count < 1)
                return null;

            PolyLineSegment polyLineSegment = new PolyLineSegment();
            PathFigure pathFigure = new PathFigure();
            pathFigure.IsClosed = isClosed;
            pathFigure.IsFilled = isFilled;
            pathFigure.StartPoint = pts[0];
            pathFigure.Segments.Add(polyLineSegment);
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            if (pts.Count < 2)
                return pathGeometry;

            else if (pts.Count == 2)
            {
                if (!isClosed)
                {
                    Segment(polyLineSegment.Points, pts[0], pts[0], pts[1], pts[1], tension, tension, tolerance);
                }
                else
                {
                    Segment(polyLineSegment.Points, pts[1], pts[0], pts[1], pts[0], tension, tension, tolerance);
                    Segment(polyLineSegment.Points, pts[0], pts[1], pts[0], pts[1], tension, tension, tolerance);
                }
            }
            else
            {
                bool useTensionCollection = tensions != null && tensions.Count > 0;

                for (int i = 0; i < pts.Count; i++)
                {
                    double T1 = useTensionCollection ? tensions[i % tensions.Count] : tension;
                    double T2 = useTensionCollection ? tensions[(i + 1) % tensions.Count] : tension;

                    if (i == 0)
                    {
                        Segment(polyLineSegment.Points, isClosed ? pts[pts.Count - 1] : pts[0],
                            pts[0], pts[1], pts[2], T1, T2, tolerance);
                    }

                    else if (i == pts.Count - 2)
                    {
                        Segment(polyLineSegment.Points, pts[i - 1], pts[i], pts[i + 1],
                            isClosed ? pts[0] : pts[i + 1], T1, T2, tolerance);
                    }

                    else if (i == pts.Count - 1)
                    {
                        if (isClosed)
                        {
                            Segment(polyLineSegment.Points, pts[i - 1], pts[i], pts[0], pts[1], T1, T2, tolerance);
                        }
                    }

                    else
                    {
                        Segment(polyLineSegment.Points, pts[i - 1], pts[i], pts[i + 1], pts[i + 2], T1, T2, tolerance);
                    }
                }
            }

            return pathGeometry;
        }

        static void Segment(PointCollection points, Point pt0, Point pt1, Point pt2, Point pt3, double T1, double T2, double tolerance)
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
                points.Add(pt);
            }
        }
    }
}