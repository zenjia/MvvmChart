using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{
    /// <summary>
    /// PathSeries just use a Path to draw the series.
    /// This is the generic series type which can be customized to create almost any shape.
    /// To achieve this, just simply pass a ISeriesGeometryBuilder object to the GeometryBuilder property.
    /// By default, the GeometryBuilder property is set to a PolyLineGeometryBuilder.
    /// </summary>
    public class PathSeries : SeriesBase
    {
        static PathSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathSeries), new FrameworkPropertyMetadata(typeof(PathSeries)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnPathDataChanged();
            //UpdatePathData();
        }

        public Geometry PathData
        {
            get { return (Geometry)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }
        public static readonly DependencyProperty PathDataProperty =
            DependencyProperty.Register("PathData", typeof(Geometry), typeof(PathSeries), new PropertyMetadata(null, OnPathDataPropertyChanged));

        private static void OnPathDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PathSeries)d).OnPathDataChanged();
        }

        private void OnPathDataChanged()
        {
            if (this.PART_Path != null)
            {
                this.PART_Path.Data = this.PathData;
            }

        }


        public ISeriesGeometryBuilder GeometryBuilder
        {
            get { return (ISeriesGeometryBuilder)GetValue(GeometryBuilderProperty); }
            set { SetValue(GeometryBuilderProperty, value); }
        }
        public static readonly DependencyProperty GeometryBuilderProperty =
            DependencyProperty.Register("GeometryBuilder", typeof(ISeriesGeometryBuilder), typeof(PathSeries), new PropertyMetadata(new PolyLineGeometryBuilder(), OnGeometryProviderPropertyChanged));

        private static void OnGeometryProviderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PathSeries)d).UpdatePathData();
        }


        protected override void UpdatePathData()
        {
            if (this.GeometryBuilder == null ||
                this.ItemsSource == null ||
                this.ItemsSource.Count == 0)
            {
                return;
            }

            var coordinates = this.GetCoordinates();  
          
            this.PathData = this.GeometryBuilder.GetGeometry(coordinates);

        }
    }
}