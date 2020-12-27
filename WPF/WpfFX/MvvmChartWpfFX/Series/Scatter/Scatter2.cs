using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using MvvmCharting.Drawing;
using MvvmCharting.Series;
using Path = System.Windows.Shapes.Path;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// Yet another Scatter type, which inherited from Shape directly.
    /// It is effectively just a path, so it is lightweight and has better performance
    /// compare to <see cref="Scatter"/> 
    /// </summary>
    [ContentProperty(nameof(Data))]
    public class Scatter2 : Shape, IScatter
    {
        #region overrides
        protected override Geometry DefiningGeometry
        {
            get
            {
                return this.Data ?? Geometry.Empty;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateAdjustedCoordinate();
        }
        #endregion

        public Scatter2()
        {

            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;

            UpdateScatterGeometry();
        }


        /// <summary>
        /// Gets or sets a <see cref="T:System.Windows.Media.Geometry" /> that specifies the shape to be drawn.
        /// </summary>
        /// <returns>A description of the shape to be drawn. </returns>
        public Geometry Data
        {
            get
            {
                return (Geometry)this.GetValue(Scatter2.DataProperty);
            }
            set
            {
                this.SetValue(Scatter2.DataProperty, (object)value);
            }
        }
        public static readonly DependencyProperty DataProperty =
            Path.DataProperty.AddOwner(typeof(Scatter2), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));



        public IScatterGeometryBuilder GeometryBuilder
        {
            get { return (IScatterGeometryBuilder)GetValue(GeometryBuilderProperty); }
            set { SetValue(GeometryBuilderProperty, value); }
        }
        public static readonly DependencyProperty GeometryBuilderProperty =
            DependencyProperty.Register("GeometryBuilder", typeof(IScatterGeometryBuilder), typeof(Scatter2), new PropertyMetadata(null, OnGeometryBuilderPropertyChanged));

        private static void OnGeometryBuilderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scatter2)d).UpdateScatterGeometry();
        }
         
        private void UpdateScatterGeometry()
        {
            if (this.GeometryBuilder == null)
            {
                return;
            }


            this.Data = (Geometry) this.GeometryBuilder?.GetGeometry();



        }


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

        /// <summary>
        /// When the render size of a Scatter is changed, we should
        /// adjust it coordinates by some offset.
        /// </summary>
        /// <returns></returns>
        public virtual PointNS GetOffsetForSizeChangedOverride()
        {
            return new PointNS(-ActualWidth / 2, -ActualHeight / 2);
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

       
    }




}