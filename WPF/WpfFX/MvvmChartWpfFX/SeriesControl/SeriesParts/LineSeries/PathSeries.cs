using System;
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
using MvvmCharting.Drawing;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX.Series
{
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
            PART_Shape = (Path)this.GetTemplateChild(sPART_Shape);


        }


        /// <summary>
        /// This should be called when GeometryBuilder, Mode or coordinates changed
        /// </summary>

        protected override void OnCoordinatesChanged(PointNS[] coordinates, PointNS[] previousCoordinates)
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
            DependencyProperty.Register("GeometryBuilder", typeof(ISeriesGeometryBuilder), typeof(PathSeries), new PropertyMetadata(new PolyLineGeometryBuilder(), OnGeometryProviderPropertyChanged));

        private static void OnGeometryProviderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PathSeries)d).UpdateShape();
        }



    }


}
