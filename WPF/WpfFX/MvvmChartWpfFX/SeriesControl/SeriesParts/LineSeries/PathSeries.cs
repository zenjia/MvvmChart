using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes; 

namespace MvvmCharting.WpfFX.Series
{
    /// <summary>
    /// Represents a LineSeries/AreaSeries which Use a <see cref="Path"/>
    /// to draw the shape. To customize the path geometry, user can implements
    /// the <see cref="ISeriesGeometryBuilder"/> and pass the object to the
    /// <see cref="GeometryBuilder"/> property,
    /// or set the PathData using mini-language in Xaml directly.
    /// </summary>
    [TemplatePart(Name = "PART_Shape", Type = typeof(Shape))]
    public class PathSeries : LineSeriesBase
    {

        private static readonly string sPART_Shape = "PART_Shape";

        protected Path PART_Shape;

        /// <summary>
        /// cache the created Geometry object
        /// </summary>
        private Geometry _pathData;
        private Geometry PathData
        {
            get { return this._pathData; }
            set
            {
                if (this._pathData != value)
                {
                    this._pathData = value;

                    if (this.PART_Shape != null)
                    {
                        this.PART_Shape.Data = value;
                    }
                }

            }

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_Shape = (Path)GetTemplateChild(sPART_Shape);


        }

        protected override void OnCoordinatesChanged(Point[] coordinates, Point[] previousCoordinates)
        {
            if (coordinates == null || coordinates.Length < 2)
            {
                this.PathData = Geometry.Empty;
                return;
            }

            this.PathData = (Geometry)this.GeometryBuilder.GetGeometry(coordinates, previousCoordinates);
        }

        protected override bool CanUpdateShape()
        {
 
            return base.CanUpdateShape() && 
                   this.GeometryBuilder != null && 
                   this.PART_Shape != null;

 
        }


        public ISeriesGeometryBuilder GeometryBuilder
        {
            get { return (ISeriesGeometryBuilder)GetValue(GeometryBuilderProperty); }
            set { SetValue(GeometryBuilderProperty, value); }
        }
        public static readonly DependencyProperty GeometryBuilderProperty =
            DependencyProperty.Register("GeometryBuilder", typeof(ISeriesGeometryBuilder), typeof(PathSeries), new PropertyMetadata(new PolylineGeometryBuilder(), OnGeometryProviderPropertyChanged));

        private static void OnGeometryProviderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PathSeries)d).UpdateShape();
        }



    }


}
