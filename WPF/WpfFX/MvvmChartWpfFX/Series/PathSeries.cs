using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MvvmCharting.Drawing;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// PathSeries just use a Path to draw the series.
    /// This is the generic series type which can be customized to create almost any shape.
    /// To achieve this, just simply pass a ISeriesGeometryBuilder object to the GeometryBuilder property.
    /// By default, the GeometryBuilder property is set to a PolyLineGeometryBuilder.
    /// </summary>
    [TemplatePart(Name = "PART_Shape", Type = typeof(Shape))]
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
        }

        /// <summary>
        /// cache the created Geometry object
        /// </summary>
        private Geometry _pathData;


        private void OnPathDataChanged()
        {
            if (this.PART_Shape != null)
            {
                ((Path)this.PART_Shape).Data = this._pathData;
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
            ((PathSeries)d).UpdateLineOrArea();
        }


        /// <summary>
        /// This should be called when GeometryBuilder, Mode or coordinates changed
        /// </summary>
        protected override void UpdateLineOrArea()
        {
            if (this.GeometryBuilder == null ||
                this.PART_Shape == null ||
                this.ItemsSource == null ||
                this.ItemsSource.Count == 0 ||
                this.RenderSize.IsInvalid() ||
                !this.IsLoaded)
            {
                return;
            }

            var coordinates = this.GetCoordinates();

            PointNS[] previous;

            switch (this.SeriesShapeType)
            {
                case SeriesShapeType.Line:
                    previous = null;
                    break;
                case SeriesShapeType.Area:
                    previous = new[] { new PointNS(0, 0), new PointNS(this.ActualWidth, 0) };
                    break;
                case SeriesShapeType.StackedArea:
                case SeriesShapeType.StackedArea100:
                    var ls = this.Owner.GetSeries().ToArray();
                    var index = Array.IndexOf(ls, this);
                    if (index == 0)
                    {
                        previous = new[] { new PointNS(0, 0), new PointNS(this.ActualWidth, 0) };
                    }
                    else
                    {
                        previous = ls[index - 1].GetCoordinates();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var geometry = coordinates == null
                ? Geometry.Empty
                : (Geometry)this.GeometryBuilder.GetGeometry(coordinates, previous);


            if (geometry != this._pathData)
            {
                this._pathData = geometry;
                OnPathDataChanged();
            }
        }
    }
}