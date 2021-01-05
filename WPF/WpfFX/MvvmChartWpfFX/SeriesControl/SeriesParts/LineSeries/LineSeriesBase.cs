using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
using MvvmCharting.Common;
using MvvmCharting.Drawing;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX.Series
{


    public interface ILineSeriesOwner
    {
        ISeriesControlOwner SeriesControlOwner { get; }
        IList ItemsSource { get; }

        double XPixelPerUnit { get; }
        double YPixelPerUnit { get; }

        PointNS[] GetCoordinates();
         
        PointNS[] GetPreviousSeriesCoordinates(bool isAreaSeries);

    }

    /// <summary>
    /// Represents a line or area series which can be displayed.
    /// Basically, it use a <see cref="Path"/> to draw the shape.
    /// If the Fill property of the path is null, then it will
    /// draw a line series(<see cref="LineSeries"/>), otherwise,
    /// it will draw a area series(<see cref="AreaSeries"/>).
    /// To customize the path geometry, user can implements the <see cref="ISeriesGeometryBuilder"/>
    /// and pass the object to the <see cref="GeometryBuilder"/> property,
    /// or set the PathData using mini-language in Xaml directly.
    /// </summary>
    [TemplatePart(Name = "PART_Shape", Type = typeof(Shape))]
    public class LineSeriesBase : InteractiveControl
    {

        private static readonly string sPART_Shape = "PART_Shape";

        protected Path PART_Shape;

        /// <summary>
        /// cache the created Geometry object
        /// </summary>
        private Geometry _pathData;
        private Geometry PathData
        {
            get { return this._pathData;}
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
        protected bool IsAreaMode { get; set; }

        internal ILineSeriesOwner Owner { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Shape = (Path)this.GetTemplateChild(sPART_Shape);

 
        }
 

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty =
            Shape.StrokeProperty.AddOwner(typeof(LineSeriesBase));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner(typeof(LineSeriesBase), new PropertyMetadata(1.0));



        public Style LineStyle
        {
            get { return (Style)GetValue(LineStyleProperty); }
            set { SetValue(LineStyleProperty, value); }
        }
        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register("LineStyle", typeof(Style), typeof(LineSeriesBase), new PropertyMetadata(null));


        /// <summary>
        /// This should be called when GeometryBuilder, Mode or coordinates changed
        /// </summary>
        internal void UpdateShape()
        {
            if (this.Owner?.SeriesControlOwner == null)
            {
                 return;
            }

            if (this.Owner.SeriesControlOwner.IsSeriesCollectionChanging)
            {
                return;
            }

            if (this.GeometryBuilder == null ||
                this.PART_Shape == null ||
                this.Owner.XPixelPerUnit.IsInvalid() ||
                this.Owner.YPixelPerUnit.IsInvalid() ||
                !this.IsLoaded)
            {
                return;
            }

 
            var coordinates = this.Owner.GetCoordinates();
            if (coordinates == null || coordinates.Length < 2)
            {
                this.PathData = Geometry.Empty;
                return;
            }
 
            PointNS[] previous = this.Owner.GetPreviousSeriesCoordinates(this.IsAreaMode);

            this.PathData = (Geometry)this.GeometryBuilder.GetGeometry(coordinates, previous);
  
        }

  

        public ISeriesGeometryBuilder GeometryBuilder
        {
            get { return (ISeriesGeometryBuilder)GetValue(GeometryBuilderProperty); }
            set { SetValue(GeometryBuilderProperty, value); }
        }
        public static readonly DependencyProperty GeometryBuilderProperty =
            DependencyProperty.Register("GeometryBuilder", typeof(ISeriesGeometryBuilder), typeof(LineSeriesBase), new PropertyMetadata(new PolyLineGeometryBuilder(), OnGeometryProviderPropertyChanged));

        private static void OnGeometryProviderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LineSeriesBase)d).UpdateShape();
        }
    }


}
