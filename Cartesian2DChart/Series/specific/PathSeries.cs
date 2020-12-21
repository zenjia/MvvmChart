
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MvvmCharting
{
    public interface IGeometryBuilder
    {
        Geometry GetGeometry(Point[] points);
    }

    /// <summary>
    /// PathSeries just use a Path to draw the series.
    /// This is the generic series type which can be customized to create almost any shape.
    /// To achieve this, just simply pass a IGeometryBuilder object to the GeometryBuilder property.
    /// By default, the GeometryBuilder property is set to a PolyLineGeometryBuilder.
    /// </summary>
    public class PathSeries : SeriesBase
    {
        static PathSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathSeries), new FrameworkPropertyMetadata(typeof(PathSeries)));
        }

        public Geometry PathData
        {
            get { return (Geometry)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }
        public static readonly DependencyProperty PathDataProperty =
            DependencyProperty.Register("PathData", typeof(Geometry), typeof(PathSeries), new PropertyMetadata(null));


        public IGeometryBuilder GeometryBuilder
        {
            get { return (IGeometryBuilder)GetValue(GeometryBuilderProperty); }
            set { SetValue(GeometryBuilderProperty, value); }
        }
        public static readonly DependencyProperty GeometryBuilderProperty =
            DependencyProperty.Register("GeometryBuilder", typeof(IGeometryBuilder), typeof(PathSeries), new PropertyMetadata(new PolyLineGeometryBuilder(), OnGeometryProviderPropertyChanged));

        private static void OnGeometryProviderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
             ((PathSeries)d).UpdateShape();
        }


        protected override void UpdateShape()
        {
            if (this.GeometryBuilder == null || this.DataPointViewModels == null)
            {
                return;
            }
            var rawPoints = this.DataPointViewModels.Select(x => x.Position).OrderBy(x => x.X).ToArray();
            this.PathData = this.GeometryBuilder.GetGeometry(rawPoints);
        }
    }
}