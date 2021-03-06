﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MvvmCharting.Common;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX.Series
{

    public interface IBarSeriesOwner
    {
        object DataContext { get; }
        ISeriesControlOwner SeriesControlOwner { get; }
        IList ItemsSource { get; }

        double XPixelPerUnit { get; }
        double YPixelPerUnit { get; }

        Point[] GetCoordinates();
        Point GetPlotCoordinateForItem(object item, int itemIndex);
        Point[] GetPreviousCoordinates(bool isAreaSeries);

    }

    public class BarSeries : InteractiveControl
    {
        static BarSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BarSeries), new FrameworkPropertyMetadata(typeof(BarSeries)));
        }

        private SlimItemsControl PART_BarItemsControl;

        internal IBarSeriesOwner Owner { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.PART_BarItemsControl != null)
            {
                this.PART_BarItemsControl.ElementGenerated -= BarItemsControlBarGenerated;
            }

            this.PART_BarItemsControl = (SlimItemsControl)GetTemplateChild("PART_BarItemsControl");
            if (this.PART_BarItemsControl != null)
            {
                this.PART_BarItemsControl.ElementGenerated += BarItemsControlBarGenerated;

                this.PART_BarItemsControl.SetBinding(SlimItemsControl.ItemTemplateSelectorProperty, new Binding(nameof(this.BarItemTemplateSelector)) { Source = this });
                this.PART_BarItemsControl.SetBinding(SlimItemsControl.ItemTemplateProperty, new Binding(nameof(this.BarItemTemplate)) { Source = this });
                UpdateBarWidth();
                UpdateItemsSource();
            }
        }

        protected virtual void BarItemsControlBarGenerated(object sender, FrameworkElement child, int childIndex)
        {
            var barItem = child as BarItem;
            if (barItem == null)
            {
                MvvmChartException.ThrowDataTemplateRootElementException(nameof(this.BarItemTemplate), typeof(BarItem));
            }

            barItem.MaxWidth = this.MaxBarWidth;

            if (this.BarBrush != null && barItem.Fill == null)
            {
                barItem.SetCurrentValue(BarItem.FillProperty, this.BarBrush);
            }

            if (this.BarBorderBrush != null && barItem.Stroke == null)
            {
                barItem.SetCurrentValue(BarItem.StrokeProperty, this.BarBorderBrush);
            }

            if (!this.BarBorderThickness.IsNaN() && barItem.StrokeThickness.IsNaN())
            {
                barItem.SetCurrentValue(BarItem.StrokeThicknessProperty, this.BarBorderThickness);
            }

            if (this.BarStyle != null && barItem.Style == null)
            {
                barItem.SetCurrentValue(StyleProperty, this.BarStyle);
            }

            if (!this.BarWidth.IsNaN() && barItem.Width.IsInvalid())
            {
                barItem.SetCurrentValue(WidthProperty, this.BarWidth);
            }


            if (!this.Owner.SeriesControlOwner.IsSeriesCollectionChanging)
            {
                if (!this.Owner.XPixelPerUnit.IsNaN() && !this.Owner.YPixelPerUnit.IsNaN())
                {
                    if (!this.BarWidth.IsNaN() && barItem.Width.IsInvalid())
                    {
                        barItem.SetCurrentValue(WidthProperty, this.BarWidth);
                    }

                    var privousCoordinates = this.Owner.GetPreviousCoordinates(false);

                    var item = barItem.DataContext;
                    var coordinate = this.Owner.GetPlotCoordinateForItem(item, childIndex);

                    var yBaseCoordinate = this.Owner.SeriesControlOwner.YBaseCoordinate;

                    double baseCoordinate = privousCoordinates == null
                        ? yBaseCoordinate
                        : privousCoordinates[childIndex].Y;

                    barItem.SetBarItemHeightAndCoordinate(coordinate, baseCoordinate);
                    
                }
            }
        }

        internal void UpdateItemsSource()
        {
            if (this.PART_BarItemsControl != null && this.Owner != null)
            {
                this.PART_BarItemsControl.ItemsSource = this.Owner.ItemsSource;
            }
        }

        public DataTemplate BarItemTemplate
        {
            get { return (DataTemplate)GetValue(BarItemTemplateProperty); }
            set { SetValue(BarItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty BarItemTemplateProperty =
            DependencyProperty.Register("BarItemTemplate", typeof(DataTemplate), typeof(BarSeries), new PropertyMetadata(null));

        public DataTemplateSelector BarItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(BarItemTemplateSelectorProperty); }
            set { SetValue(BarItemTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty BarItemTemplateSelectorProperty =
            DependencyProperty.Register("BarItemTemplateSelector", typeof(DataTemplateSelector), typeof(BarSeries), new PropertyMetadata(null));




        public Brush BarBrush
        {
            get { return (Brush)GetValue(BarBrushProperty); }
            set { SetValue(BarBrushProperty, value); }
        }
        public static readonly DependencyProperty BarBrushProperty =
            DependencyProperty.Register("BarBrush", typeof(Brush), typeof(BarSeries), new PropertyMetadata(null, OnBarBrushPropertyChanged));
        private static void OnBarBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BarSeries)d).OnBarBrushChanged();
        }

        protected virtual void OnBarBrushChanged()
        {

            if (this.PART_BarItemsControl != null)
            {
                foreach (var barItem in this.PART_BarItemsControl.GetChildren().OfType<BarItem>())
                {
                    var vs = DependencyPropertyHelper.GetValueSource(barItem, BarItem.FillProperty);
                    if (vs.IsCurrent)
                    {
                        barItem.SetCurrentValue(BarItem.FillProperty, this.BarBrush);
                    }

                }
            }
        }


        public Brush BarBorderBrush
        {
            get { return (Brush)GetValue(BarBorderBrushProperty); }
            set { SetValue(BarBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty BarBorderBrushProperty =
            DependencyProperty.Register("BarBorderBrush", typeof(Brush), typeof(BarSeries), new PropertyMetadata(null, OnBarBorderBrushPropertyChanged));

        private static void OnBarBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BarSeries)d).OnBarBorderBrushChanged();
        }

        protected virtual void OnBarBorderBrushChanged()
        {


            if (this.PART_BarItemsControl != null)
            {
                foreach (var barItem in this.PART_BarItemsControl.GetChildren().OfType<BarItem>())
                {
                    var vs = DependencyPropertyHelper.GetValueSource(barItem, BarItem.StrokeProperty);
                    if (vs.IsCurrent)
                    {
                        barItem.SetCurrentValue(BarItem.StrokeProperty, this.BarBorderBrush);
                    }

                }
            }
        }

        public double BarBorderThickness
        {
            get { return (double)GetValue(BarBorderThicknessProperty); }
            set { SetValue(BarBorderThicknessProperty, value); }
        }
        public static readonly DependencyProperty BarBorderThicknessProperty =
            DependencyProperty.Register("BarBorderThickness", typeof(double), typeof(BarSeries), new PropertyMetadata(double.NaN, OnBarBorderThicknessPropertyChanged));
        private static void OnBarBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BarSeries)d).OnBarBorderThicknessChanged();
        }

        protected virtual void OnBarBorderThicknessChanged()
        {
            if (this.PART_BarItemsControl != null)
            {
                foreach (var barItem in this.PART_BarItemsControl.GetChildren().OfType<BarItem>())
                {
                    var vs = DependencyPropertyHelper.GetValueSource(barItem, BarItem.StrokeThicknessProperty);
                    if (vs.IsCurrent)
                    {
                        barItem.SetCurrentValue(BarItem.StrokeThicknessProperty, this.BarBorderThickness);
                    }

                }
            }
        }

        public Style BarStyle
        {
            get { return (Style)GetValue(BarStyleProperty); }
            set { SetValue(BarStyleProperty, value); }
        }
        public static readonly DependencyProperty BarStyleProperty =
            DependencyProperty.Register("BarStyle", typeof(Style), typeof(BarSeries), new PropertyMetadata(null, OnBarStyleThicknessPropertyChanged));
        private static void OnBarStyleThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BarSeries)d).OnBarStyleThicknessChanged();
        }

        protected virtual void OnBarStyleThicknessChanged()
        {
            if (this.PART_BarItemsControl != null)
            {
                foreach (var barItem in this.PART_BarItemsControl.GetChildren().OfType<BarItem>())
                {
                    var vs = DependencyPropertyHelper.GetValueSource(barItem, StyleProperty);
                    if (vs.IsCurrent)
                    {
                        barItem.SetCurrentValue(StyleProperty, this.BarStyle);
                    }

                }
            }
        }

        /// <summary>
        /// user-set bar width
        /// </summary>
        public double BarWidth
        {
            get { return (double)GetValue(BarWidthProperty); }
            set { SetValue(BarWidthProperty, value); }
        }
        public static readonly DependencyProperty BarWidthProperty =
            DependencyProperty.Register("BarWidth", typeof(double), typeof(BarSeries), new PropertyMetadata(double.NaN, OnBarWidthPropertyChanged));

        private static void OnBarWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BarSeries)d).OnBarWidthChanged();
        }
        private void OnBarWidthChanged()
        {
            UpdateBarWidth();
        }

        //private double _barWidthInternal = double.NaN;
        ///// <summary>
        ///// calculated bar width
        ///// </summary>
        //internal double BarWidthInternal
        //{
        //    get { return this._barWidthInternal; }
        //    set
        //    {
        //        if (this._barWidthInternal != value)
        //        {

        //            this._barWidthInternal = value;
        //            UpdateBarItemWidth();
        //        }

        //    }
        //}

        //private double GetProperBarWidth()
        //{
        //    double w = this.BarWidth;
        //    if (w.IsNaN())
        //    {
        //        w = this.BarWidthInternal;
        //    }

        //    return w;
        //}

        private void UpdateBarWidth()
        {
            if (this.PART_BarItemsControl == null)
            {
                return;
            }


            foreach (var barItem in this.PART_BarItemsControl.GetChildren().OfType<BarItem>())
            {
                var vs = DependencyPropertyHelper.GetValueSource(barItem, WidthProperty);
                if (vs.IsCurrent)
                {
                    barItem.SetCurrentValue(WidthProperty, this.BarWidth);
                }

            }
        }


        internal void SetBarWidth(double width)
        {
            if (this.BarWidth.IsNaN() ||
                DependencyPropertyHelper.GetValueSource(this, BarWidthProperty).IsCurrent)
            {
                SetCurrentValue(BarWidthProperty, width);
            }
        }

        internal void UpdateBarWidth(double minXGap, double xPixelPerUnit)
        {
            if (minXGap.IsNaN() || xPixelPerUnit.IsInvalid())
            {
                return;
            }

            double width = minXGap * xPixelPerUnit;

            SetBarWidth(width);
        }


        public double MaxBarWidth
        {
            get { return (double)GetValue(MaxBarWidthProperty); }
            set { SetValue(MaxBarWidthProperty, value); }
        }
        public static readonly DependencyProperty MaxBarWidthProperty =
            DependencyProperty.Register("MaxBarWidth", typeof(double), typeof(BarSeries), new PropertyMetadata(double.NaN, OnMaxBarWidthPropertyChanged));

        private static void OnMaxBarWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BarSeries)d).OnMaxBarWidthChanged();
        }
        private void OnMaxBarWidthChanged()
        {
            if (this.PART_BarItemsControl == null ||
                this.MaxBarWidth.IsNaN())
            {
                return;
            }


            foreach (var barItem in this.PART_BarItemsControl.GetChildren().OfType<BarItem>())
            {
                barItem.MaxWidth = this.MaxBarWidth;
            }
        }



        internal void UpdateBarCoordinateAndHeight(object item, Point coordinate, double previousYCoordinate)
        {
            var barItem = (BarItem)this.PART_BarItemsControl?.TryGetChildForItem(item);

            barItem?.SetBarItemHeightAndCoordinate(coordinate, previousYCoordinate);



        }



    }
}
