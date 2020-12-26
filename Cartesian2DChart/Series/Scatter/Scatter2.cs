using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MvvmCharting
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



        public Point Coordinate
        {
            get { return (Point)GetValue(CoordinateProperty); }
            set { SetValue(CoordinateProperty, value); }
        }
        public static readonly DependencyProperty CoordinateProperty =
            DependencyProperty.Register("Coordinate", typeof(Point), typeof(Scatter2), new PropertyMetadata(default(Point), OnCoordinatePropertyChanged));

        private static void OnCoordinatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scatter2)d).OnCoordinateChanged((Point)e.NewValue);
        }
        private void OnCoordinateChanged(Point newValue)
        {
            UpdateAdjustedCoordinate();

        }


        private void UpdateAdjustedCoordinate()
        {
            if (this.Coordinate.IsEmpty())
            {
                return;
            }

            var offset = GetOffsetForSizeChangedOverride(this.RenderSize);

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


        public Point GetOffsetForSizeChangedOverride(Size newSize)
        {

            return new Point(-newSize.Width / 2, -newSize.Height / 2);
        }
    }
}
