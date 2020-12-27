using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MvvmCharting.Drawing;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{
   
    public class Scatter2 : Control, IScatter
    {
        static Scatter2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Scatter2), new FrameworkPropertyMetadata(typeof(Scatter2)));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateAdjustedCoordinate();
        }

        public Scatter2()
        {
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;
        }


        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty =
            Shape.StrokeProperty.AddOwner(typeof(Scatter2));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner(typeof(Scatter2));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            Shape.FillProperty.AddOwner(typeof(Scatter2));



        public PointNS Coordinate
        {
            get { return (PointNS)GetValue(CoordinateProperty); }
            set { SetValue(CoordinateProperty, value); }
        }
        public static readonly DependencyProperty CoordinateProperty =
            DependencyProperty.Register("Coordinate", typeof(PointNS), typeof(Scatter2), new PropertyMetadata(PointNSHelper.EmptyPoint, OnCoordinatePropertyChanged));

        private static void OnCoordinatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scatter2)d).OnCoordinateChanged((PointNS)e.NewValue);
        }
        private void OnCoordinateChanged(PointNS newValue)
        {
            UpdateAdjustedCoordinate();

        }


        private void UpdateAdjustedCoordinate()
        {
            if (this.Coordinate.IsEmpty())
            {
                return;
            }

            var offset = GetOffsetForSizeChangedOverride();

            if (offset.IsEmpty())
            {
                return;
            }

            var x = this.Coordinate.X + offset.X;
            var y = this.Coordinate.Y + offset.Y;


            //if (!double.IsInfinity(x))
            //{
            //    Canvas.SetLeft(this, x);
            //}


            //if (!double.IsInfinity(y))
            //{
            //    Canvas.SetTop(this, y);
            //}

            var translateTransform = this.RenderTransform as TranslateTransform;
            if (translateTransform == null)
            {
                this.RenderTransform = new TranslateTransform(x, y);
            }
            else
            {
                translateTransform.Y = y;
                translateTransform.X = x;
            }
        }


        public PointNS GetOffsetForSizeChangedOverride()
        {
            //
            return new PointNS(/*-ActualWidth / 2, -ActualHeight / 2*/);
        }
    }
}
