﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MvvmCharting.Common;
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
            if (this.Owner.IsSeriesCollectionChanging)
            {
                return;
            }

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
            if (coordinates.Length < 2)
            {
                this._pathData = Geometry.Empty;
                OnPathDataChanged();
                return;
            }

            PointNS[] previous;

            switch (this.SeriesMode)
            {
                case WpfFX.SeriesMode.Line:
                    previous = null;
                    break;
                case WpfFX.SeriesMode.Area:
                    previous = new[] { new PointNS(coordinates.First().X, 0), new PointNS(coordinates.Last().X, 0) };
                    break;
                case WpfFX.SeriesMode.StackedArea:
                case WpfFX.SeriesMode.StackedArea100:
                    var ls = this.Owner.GetSeries().ToArray();
                    var index = Array.IndexOf(ls, this);
                    if (index == 0)
                    {
                        previous = new[] { new PointNS(coordinates.First().X, 0), new PointNS(coordinates.Last().X, 0) };
                    }
                    else
                    {
                        previous = ls[index - 1].GetCoordinates();

                        if (previous.Length != coordinates.Length)
                        {
                            throw new MvvmChartException($"previous.Length({previous.Length}) != coordinates.Length({coordinates.Length})");
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var geometry = coordinates == null
                ? Geometry.Empty
                : (Geometry)this.GeometryBuilder.GetGeometry(coordinates, previous);


            this._pathData = geometry;
            OnPathDataChanged();
        }
    }
}