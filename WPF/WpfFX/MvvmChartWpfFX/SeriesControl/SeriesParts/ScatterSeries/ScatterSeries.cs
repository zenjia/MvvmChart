using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MvvmCharting.Common; 

namespace MvvmCharting.WpfFX.Series
{

    public interface IScatterSeriesOwner
    {
        ISeriesControlOwner SeriesControlOwner { get; }
        IList ItemsSource { get; }

        double XPixelPerUnit { get; }
        double YPixelPerUnit { get; }

        Point[] GetCoordinates();
        Point GetPlotCoordinateForItem(object item, int itemIndex);
    }

    /// <summary>
    /// Represents a collection of scatters, and each scatter represents an item.
    /// </summary>
    [TemplatePart(Name = "PART_ScatterItemsControl", Type = typeof(SlimItemsControl))]
    public class ScatterSeries : InteractiveControl
    {
        static ScatterSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScatterSeries), new FrameworkPropertyMetadata(typeof(ScatterSeries)));
        }

        private static readonly string sPART_ScatterItemsControl = "PART_ScatterItemsControl";
        private SlimItemsControl PART_ScatterItemsControl;


        internal IScatterSeriesOwner Owner { get; set; }



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.PART_ScatterItemsControl != null)
            {
                this.PART_ScatterItemsControl.ElementGenerated -= ScatterItemsControlScatterGenerated;

            }

            this.PART_ScatterItemsControl = (SlimItemsControl)GetTemplateChild(sPART_ScatterItemsControl);
            if (this.PART_ScatterItemsControl != null)
            {
                this.PART_ScatterItemsControl.ElementGenerated += ScatterItemsControlScatterGenerated;

                this.PART_ScatterItemsControl.SetBinding(SlimItemsControl.ItemTemplateSelectorProperty, new Binding(nameof(this.ScatterTemplateSelector)) { Source = this });
                this.PART_ScatterItemsControl.SetBinding(SlimItemsControl.ItemTemplateProperty, new Binding(nameof(this.ScatterTemplate)) { Source = this });

                UpdateItemsSource();

            }



        }

        protected virtual void ScatterItemsControlScatterGenerated(object sender, FrameworkElement root, int index)
        {
            var scatter = root as Scatter;
            if (scatter == null)
            {
                MvvmChartException.ThrowDataTemplateRootElementException(nameof(this.ScatterTemplate), typeof(Scatter));
            }



            if (this.ScatterBrush != null && scatter.Fill == null)
            {
                scatter.SetCurrentValue(Scatter.FillProperty, this.ScatterBrush);
            }

            var item = scatter.DataContext;

            if (!this.Owner.SeriesControlOwner.IsSeriesCollectionChanging)
            {
                if (!this.Owner.XPixelPerUnit.IsNaN() && !this.Owner.YPixelPerUnit.IsNaN())
                {
                    scatter.Coordinate = this.Owner.GetPlotCoordinateForItem(item, index);
                }
            }
        }

        public DataTemplate ScatterTemplate
        {
            get { return (DataTemplate)GetValue(ScatterTemplateProperty); }
            set { SetValue(ScatterTemplateProperty, value); }
        }
        public static readonly DependencyProperty ScatterTemplateProperty =
            DependencyProperty.Register("ScatterTemplate", typeof(DataTemplate), typeof(ScatterSeries), new PropertyMetadata(null));

        public DataTemplateSelector ScatterTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ScatterTemplateSelectorProperty); }
            set { SetValue(ScatterTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty ScatterTemplateSelectorProperty =
            DependencyProperty.Register("ScatterTemplateSelector", typeof(DataTemplateSelector), typeof(ScatterSeries), new PropertyMetadata(null));

        public Brush ScatterBrush
        {
            get { return (Brush)GetValue(ScatterBrushProperty); }
            set { SetValue(ScatterBrushProperty, value); }
        }
        public static readonly DependencyProperty ScatterBrushProperty =
            DependencyProperty.Register("ScatterBrush", typeof(Brush), typeof(ScatterSeries), new PropertyMetadata(null, OnScatterBrushPropertyChanged));

        private static void OnScatterBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //((ScatterSeries)d).OnScatterBrushChanged();
        }

        private void OnScatterBrushChanged()
        {

            if (this.PART_ScatterItemsControl == null)
            {
                return;
            }

            foreach (var scatter in this.PART_ScatterItemsControl.GetChildren().OfType<Scatter>())
            {
                if (this.ScatterBrush == null || 
                    DependencyPropertyHelper.GetValueSource(scatter, Scatter.FillProperty).IsCurrent)
                {
                    scatter.SetCurrentValue(Scatter.FillProperty, this.ScatterBrush);
                }
            }

        }
        internal void UpdateItemsSource()
        {
            if (this.PART_ScatterItemsControl != null && this.Owner != null)
            {
                this.PART_ScatterItemsControl.ItemsSource = this.Owner.ItemsSource;
            }
        }

        internal void UpdateScatterCoordinate(object item, Point coordinate)
        {
            var scatter = (Scatter)this.PART_ScatterItemsControl?.TryGetChildForItem(item);
            if (scatter != null)
            {
                scatter.Coordinate = coordinate;
            }

        }

        internal void UpdateCoordinate()
        {
            if (this.PART_ScatterItemsControl == null)
            {
                return;
            }

            var coordinates = this.Owner.GetCoordinates();

            if (coordinates == null)
            {

                return;
            }

            if (coordinates.Length != this.PART_ScatterItemsControl.ItemCount)
            {
                throw new NotImplementedException();
            }

            for (int i = 0; i < coordinates.Length; i++)
            {
                var item = this.PART_ScatterItemsControl.ItemsSource[i];
                UpdateScatterCoordinate(item, coordinates[i]);
            }


        }





    }
}
