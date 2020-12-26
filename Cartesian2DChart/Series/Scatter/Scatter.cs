using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

namespace MvvmCharting
{
    /// <summary>
    /// This is used to display a point(dot) for an Item on the plotting area.
    /// On a Series, items scatters can be displayed on the line(curve).
    /// Each scatter represents an item, indicating the position of the item in
    /// the series.
    /// </summary>
    [ContentProperty(nameof(Data))]
    public class Scatter : Shape, IScatter
    {

        /// <summary>
        /// Gets or sets a <see cref="T:System.Windows.Media.Geometry" /> that specifies the shape to be drawn.
        /// </summary>
        /// <returns>A description of the shape to be drawn. </returns>
        public Geometry Data
        {
            get
            {
                return (Geometry)this.GetValue(Scatter.DataProperty);
            }
            set
            {
                this.SetValue(Scatter.DataProperty, (object)value);
            }
        }
        public static readonly DependencyProperty DataProperty =
            Path.DataProperty.AddOwner(typeof(Scatter), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        protected override Geometry DefiningGeometry
        {
            get
            {
                
                return this.Data ?? Geometry.Empty;
            }
        }

        

        public IScatterGeometryBuilder GeometryBuilder
        {
            get { return (IScatterGeometryBuilder)GetValue(GeometryBuilderProperty); }
            set { SetValue(GeometryBuilderProperty, value); }
        }
        public static readonly DependencyProperty GeometryBuilderProperty =
            DependencyProperty.Register("GeometryBuilder", typeof(IScatterGeometryBuilder), typeof(Scatter), new PropertyMetadata(null, OnGeometryBuilderPropertyChanged));



        private static void OnGeometryBuilderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scatter)d).UpdateScatterGeometry();
        }
         
        private void UpdateScatterGeometry()
        {
            if (this.GeometryBuilder == null)
            {
                return;
            }


            this.Data = this.GeometryBuilder?.GetGeometry();



        }

        public Scatter()
        {

            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;

            UpdateScatterGeometry();
        }



        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateAdjustedCoordinate();
        }

        
        public Point Coordinate
        {
            get { return (Point)GetValue(CoordinateProperty); }
            set { SetValue(CoordinateProperty, value); }
        }
        public static readonly DependencyProperty CoordinateProperty =
            DependencyProperty.Register("Coordinate", typeof(Point), typeof(Scatter), new PropertyMetadata(PointHelper.EmptyPoint, OnCoordinatePropertyChanged));

        private static void OnCoordinatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scatter)d).OnCoordinateChanged((Point)e.NewValue);
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


        public virtual Point GetOffsetForSizeChangedOverride(Size newSize)
        {
            return new Point(-newSize.Width / 2, -newSize.Height / 2);
        }
    }




}