using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MvvmCharting.Common; 

namespace MvvmCharting.WpfFX.Series
{
 

    public class BarItem : InteractiveControl, IPlottable_2D
    {
        static BarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BarItem), new FrameworkPropertyMetadata(typeof(BarItem)));
        }

        private Point _coordinate = PointHelper.Empty;
        public Point Coordinate
        {
            get { return this._coordinate; }
            set
            {
                if (this._coordinate != value)
                {
                    this._coordinate = value;
                    UpdatePosition();
                }

            }
        }
        public BarItem()
        {
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdatePosition();

        }

        private void UpdatePosition()
        {
            if (this.Coordinate.IsEmpty() || this.RenderSize.IsInvalid())
            {
                return;
            }

            double xOffset = GetXOffsetForSizeChangedOverride();
            if (xOffset.IsNaN())
            {
                return;
            }

            var x = this.Coordinate.X + xOffset;

          
            var y = this.Coordinate.Y -  this.ActualHeight;

            var translateTransform = this.RenderTransform as TranslateTransform;
            if (translateTransform == null)
            {
                this.RenderTransform = new TranslateTransform(x, y);
            }
            else
            {
                translateTransform.X = x;
                translateTransform.Y = y;
            }

        }

        public double GetXOffsetForSizeChangedOverride()
        {
            return -(this.Margin.Left + this.Margin.Right + this.ActualWidth) * 0.5;
        }

        public void SetBarHeight(double newValue)
        {
            if (newValue.IsInvalid())
            {
                return;
            }

            SetCurrentValue(HeightProperty, newValue);

            UpdatePosition();
        }

 

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty =
            Shape.StrokeProperty.AddOwner(typeof(BarItem));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner(typeof(BarItem));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            Shape.FillProperty.AddOwner(typeof(BarItem));

        public Style BarStyle
        {
            get { return (Style)GetValue(BarStyleProperty); }
            set { SetValue(BarStyleProperty, value); }
        }
        public static readonly DependencyProperty BarStyleProperty =
            DependencyProperty.Register("BarStyle", typeof(Style), typeof(BarItem), new PropertyMetadata(null));

 
        

    }
}