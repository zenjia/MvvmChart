using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using MvvmCharting.Common;
using MvvmCharting.Drawing;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// The base class for all series. It implements <see cref="ISeries"/> so it
    /// can be put at the root of the SeriesTemplate of a <see cref="Chart"/>
    /// </summary>
    [TemplatePart(Name = "PART_DataPointItemsControl", Type = typeof(SlimItemsControl))]
    [TemplatePart(Name = "PART_Path", Type = typeof(Path))]
    public abstract class SeriesBase : Control, ISeries
    {

        private static readonly string sPART_Path = "PART_Path";
        private static readonly string sPART_ScatterItemsControl = "PART_DataPointItemsControl";

        public event Action<Range> XRangeChanged;
        public event Action<Range> YRangeChanged;

        public event Action<object, string> PropertyChanged;

        private SlimItemsControl PART_ScatterItemsControl;
        protected Path PART_Path { get; private set; }

        protected SeriesBase()
        {
            this.Loaded += SeriesBase_Loaded;

        }

        private void SeriesBase_Loaded(object sender, RoutedEventArgs e)
        {
         
            UpdateScattersCoordinate();
        }

        #region overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Path = (Path)GetTemplateChild(sPART_Path);
            if (this.PART_Path != null)
            {
                this.PART_Path.Visibility = this.IsLineOrAreaVisible ? Visibility.Visible : Visibility.Collapsed;
                UpdatePathFill();
            }


            if (this.PART_ScatterItemsControl != null)
            {
                this.PART_ScatterItemsControl.ElementGenerated -= ScatterItemsControlScatterGenerated;

            }

            this.PART_ScatterItemsControl = (SlimItemsControl)GetTemplateChild(sPART_ScatterItemsControl);
            if (this.PART_ScatterItemsControl != null)
            {
                this.PART_ScatterItemsControl.ElementGenerated += ScatterItemsControlScatterGenerated;
                this.PART_ScatterItemsControl.Visibility = this.IsScatterVisible ? Visibility.Visible : Visibility.Collapsed;
                this.PART_ScatterItemsControl.ItemsSource = this.ItemsSource;
                this.PART_ScatterItemsControl.ItemTemplateSelector = this.ScatterTemplateSelector;
                this.PART_ScatterItemsControl.ItemTemplate = this.ScatterTemplate;

                //UpdateScattersCoordinate();
            }



        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (sizeInfo.WidthChanged)
            {
                UpdatePixelPerUnit(Orientation.Horizontal);
            }

            if (sizeInfo.HeightChanged)
            {
                UpdatePixelPerUnit(Orientation.Vertical);
            }

       
            UpdateScattersCoordinate();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == Control.IsMouseOverProperty)
            {
                this.IsHighLighted = this.IsMouseOver;
            }
            else if (e.Property == SeriesBase.IsHighLightedProperty)
            {
                this.PropertyChanged?.Invoke(this, nameof(this.IsHighLighted));
            }


        }
        #endregion

        private void ScatterItemsControlScatterGenerated(object sender, DependencyObject root)
        {
            var scatter = (IScatter)root;
            if (scatter == null)
            {
                throw new MvvmChartException("The root element in the ScatterTemplate must implement IScatter interface.");
            }

            var item = scatter.DataContext;


            if (!this.xPixelPerUnit.IsNaN() && !this.yPixelPerUnit.IsNaN())
            {
                scatter.Coordinate = GetPlotCoordinateForItem(item);

            }




        }


        #region IndependentValueProperty & DependentValueProperty properties
        public string IndependentValueProperty
        {
            get { return (string)GetValue(IndependentValuePropertyProperty); }
            set { SetValue(IndependentValuePropertyProperty, value); }
        }
        public static readonly DependencyProperty IndependentValuePropertyProperty =
            DependencyProperty.Register("IndependentValueProperty", typeof(string), typeof(SeriesBase), new PropertyMetadata(null));


        public string DependentValueProperty
        {
            get { return (string)GetValue(DependentValuePropertyProperty); }
            set { SetValue(DependentValuePropertyProperty, value); }
        }
        public static readonly DependencyProperty DependentValuePropertyProperty =
            DependencyProperty.Register("DependentValueProperty", typeof(string), typeof(SeriesBase), new PropertyMetadata(null));
        #endregion

        #region IsScatterVisible & IsLineOrAreaVisible properties
        public bool IsScatterVisible
        {
            get { return (bool)GetValue(IsScatterVisibleProperty); }
            set { SetValue(IsScatterVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsScatterVisibleProperty =
            DependencyProperty.Register("IsScatterVisible", typeof(bool), typeof(SeriesBase), new PropertyMetadata(true, OnIsSeriesPointsVisiblePropertyChanged));

        private static void OnIsSeriesPointsVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesBase)d).OnIsSeriesPointsVisibleChanged();
        }

        private void OnIsSeriesPointsVisibleChanged()
        {
            if (this.PART_ScatterItemsControl != null)
            {

                this.PART_ScatterItemsControl.Visibility = this.IsScatterVisible ? Visibility.Visible : Visibility.Collapsed;

            }
        }

        public bool IsLineOrAreaVisible
        {
            get { return (bool)GetValue(IsLineOrAreaVisibleProperty); }
            set { SetValue(IsLineOrAreaVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsLineOrAreaVisibleProperty =
            DependencyProperty.Register("IsLineOrAreaVisible", typeof(bool), typeof(SeriesBase), new PropertyMetadata(true, OnIsLineOrAreaVisiblePropertyChanged));

        private static void OnIsLineOrAreaVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesBase)d).OnIsLineOrAreaVisibleChanged();
        }

        private void OnIsLineOrAreaVisibleChanged()
        {
            if (this.PART_Path != null)
            {
                this.PART_Path.Visibility = this.IsLineOrAreaVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region Path properties
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty =
            Shape.StrokeProperty.AddOwner(typeof(SeriesBase));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner(typeof(SeriesBase), new PropertyMetadata(1.0));

        public bool IsHighLighted
        {
            get { return (bool)GetValue(IsHighLightedProperty); }
            set { SetValue(IsHighLightedProperty, value); }
        }
        public static readonly DependencyProperty IsHighLightedProperty =
            DependencyProperty.Register("IsHighLighted", typeof(bool), typeof(SeriesBase), new PropertyMetadata(false));



        #endregion

        #region ItemsSource property and handlers


        /// <summary>
        /// Represents the data for a series.
        /// Currently can only handle numerical(& DateTime, DataTimeOffset) data.
        /// NOTE: It will be the user's responsibility to keep the ItemsSource sorted
        /// by Independent property(x value) ascendingly! 
        /// </summary>
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(SeriesBase), new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SeriesBase c = (SeriesBase)d;


            c.OnItemsSourceChanged((IList)e.OldValue, (IList)e.NewValue);

        }



        private void OnItemsSourceChanged(IList oldValue, IList newValue)
        {
            if (this.PART_ScatterItemsControl != null)
            {
                this.PART_ScatterItemsControl.ItemsSource = this.ItemsSource;
            }


            if (oldValue is INotifyCollectionChanged oldItemsSource)
            {
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .RemoveHandler(oldItemsSource, "CollectionChanged", ItemsSource_CollectionChanged);
            }

            if (newValue != null)
            {

                HandleItemsSourceCollectionChange(null, newValue);

                if (newValue is INotifyCollectionChanged newItemsSource)
                {
                    WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                        .AddHandler(newItemsSource, "CollectionChanged", ItemsSource_CollectionChanged);
                }
            }

        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //this._dataPointViewModels?.Clear();
            }

            HandleItemsSourceCollectionChange(e.OldItems, e.NewItems);
        }

        protected virtual void HandleItemsSourceCollectionChange(IList oldValue, IList newValue)
        {

            UpdateValueRange();
             
            UpdateScattersCoordinate();
        }

        public void UpdateValueRange()
        {
            if (this.ItemsSource == null ||
                this.ItemsSource.Count == 0)
            {
                this.XValueRange = Range.Empty;
                this.YValueRange = Range.Empty;

                return;
            }


            double minX = double.MaxValue, minY = double.MaxValue,
                maxX = double.MinValue, maxY = double.MinValue;

            foreach (var item in this.ItemsSource)
            {
                var pt = GetValueFromItem(item);

                var x = pt.X;
                var y = pt.Y;


                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);

                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);

            }

            this.XValueRange = new Range(minX, maxX);
            this.YValueRange = new Range(minY, maxY);
        }


        protected double GetXValueFromItem(object item)
        {
            var t = item.GetType();
            var x = t.GetProperty(this.IndependentValueProperty).GetValue(item);

            return DoubleValueConverter.ObjectToDouble(x);
        }

        protected double GetYValueFromItem(object item)
        {
            var t = item.GetType();

            var y = t.GetProperty(this.DependentValueProperty).GetValue(item);

            return DoubleValueConverter.ObjectToDouble(y);

        }

        protected Point GetValueFromItem(object item)
        {
            var t = item.GetType();
            var x = t.GetProperty(this.IndependentValueProperty).GetValue(item);
            var y = t.GetProperty(this.DependentValueProperty).GetValue(item);

            var pt = new Point(DoubleValueConverter.ObjectToDouble(x), DoubleValueConverter.ObjectToDouble(y));
            return pt;
        }

        #endregion

        #region Mode
        public SeriesGeometryMode Mode
        {
            get { return (SeriesGeometryMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(SeriesGeometryMode), typeof(SeriesBase), new PropertyMetadata(SeriesGeometryMode.Line, OnModePropertyChanged));

        private static void OnModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PathSeries)d).OnModeChanged();
        }

        private void OnModeChanged()
        {
            UpdatePathFill();
            UpdateLineOrArea();
        }

        private void UpdatePathFill()
        {
            if (this.PART_Path != null)
            {
                if (this.Mode == SeriesGeometryMode.Line)
                {
                    this.PART_Path.Fill = null;
                }
                else
                {
                    this.PART_Path.SetBinding(Shape.FillProperty, new Binding(nameof(this.Stroke)) { Source = this });
                }

            }
        }
        #endregion

        #region ItemTemplate & ItemTemplateSelector properties
        public DataTemplate ScatterTemplate
        {
            get { return (DataTemplate)GetValue(ScatterTemplateProperty); }
            set { SetValue(ScatterTemplateProperty, value); }
        }
        public static readonly DependencyProperty ScatterTemplateProperty =
            DependencyProperty.Register("ScatterTemplate", typeof(DataTemplate), typeof(SeriesBase), new PropertyMetadata(null, OnScatterTemplatePropertyChanged));

        private static void OnScatterTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesBase)d).OnScatterTemplatePropertyChanged();

        }

        private void OnScatterTemplatePropertyChanged()
        {

            if (this.PART_ScatterItemsControl != null)
            {
                this.PART_ScatterItemsControl.ItemTemplate = this.ScatterTemplate;
            }

        }

        public DataTemplateSelector ScatterTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ScatterTemplateSelectorProperty); }
            set { SetValue(ScatterTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty ScatterTemplateSelectorProperty =
            DependencyProperty.Register("ScatterTemplateSelector", typeof(DataTemplateSelector), typeof(SeriesBase), new PropertyMetadata(null, OnScatterDataTemplateSelectorPropertyChanged));

        private static void OnScatterDataTemplateSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesBase)d).OnScatterDataTemplateSelectorChanged();
        }

        private void OnScatterDataTemplateSelectorChanged()
        {
            if (this.PART_ScatterItemsControl != null)
            {
                this.PART_ScatterItemsControl.ItemTemplateSelector = this.ScatterTemplateSelector;
            }
        }
        #endregion


        #region value range
        private Range _yValueRange = Range.Empty;
        /// <summary>
        /// The min & max of the dependent value
        /// </summary>
        public Range YValueRange
        {
            get { return this._yValueRange; }
            private set
            {
                if (this._yValueRange != value)
                {
                    this._yValueRange = value;

                    this.YRangeChanged?.Invoke(this.YValueRange);

                }

            }
        }

        private Range _xValueRange = Range.Empty;
        /// <summary>
        /// The min & max of the dependent value
        /// </summary>
        public Range XValueRange
        {
            get { return this._xValueRange; }
            private set
            {
                if (this._xValueRange != value)
                {
                    this._xValueRange = value;
                    this.XRangeChanged?.Invoke(this.XValueRange);


                }
            }
        }
        #endregion

        #region plotting value range
        private Range _plottingXValueRange = Range.Empty;
        /// <summary>
        /// The final X value range used to plot the chart
        /// </summary>
        public Range PlottingXValueRange
        {
            get { return this._plottingXValueRange; }
            set
            {
                if (this._plottingXValueRange != value)
                {
                    this._plottingXValueRange = value;
                    UpdatePixelPerUnit(Orientation.Horizontal);
                     
                    UpdateScattersCoordinate();
                }
            }
        }

        private Range _plottingYValueRange = Range.Empty;
        /// <summary>
        /// The final Y value range used to plot the chart
        /// </summary>
        public Range PlottingYValueRange
        {
            get { return this._plottingYValueRange; }
            set
            {
                if (this._plottingYValueRange != value)
                {
                    this._plottingYValueRange = value;
                    UpdatePixelPerUnit(Orientation.Vertical);


                    UpdateScattersCoordinate();

                }
            }
        }
        #endregion


        #region Coordinates calculating
        protected double xPixelPerUnit { get; private set; }
        protected double yPixelPerUnit { get; private set; }

        private void UpdatePixelPerUnit(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:

                    if (this.PlottingXValueRange.IsInvalid ||
                        this.RenderSize.IsInvalid())
                    {
                        this.xPixelPerUnit = double.NaN;
                        return;
                    }

                    this.xPixelPerUnit = this.RenderSize.Width / this.PlottingXValueRange.Span;
                    break;
                case Orientation.Vertical:

                    if (this.PlottingYValueRange.IsInvalid ||
                        this.RenderSize.IsInvalid())
                    {
                        this.yPixelPerUnit = double.NaN;

                        return;
                    }

                    this.yPixelPerUnit = this.RenderSize.Height / this.PlottingYValueRange.Span;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }


        }

        protected virtual PointNS GetPlotCoordinateForItem(object item)
        {
            var itemValue = GetValueFromItem(item);
            var pt = new PointNS((itemValue.X - this.PlottingXValueRange.Min) * this.xPixelPerUnit,
                (itemValue.Y - this.PlottingYValueRange.Min) * this.yPixelPerUnit);

            return pt;
        }

        /// <summary>
        /// This should be call when: 1) ItemsSource; 2) x or y PixelPerUnit, and 3) RenderSize changed.
        /// This method will first update the _coordinateCache and the Coordinate of each scatter,
        /// then update the shape of the Line or Area
        /// </summary>
        private void UpdateScattersCoordinate()
        {
            if (this.xPixelPerUnit.IsNaN() ||
                this.yPixelPerUnit.IsNaN() ||
                this.ItemsSource == null ||
                this.ItemsSource.Count == 0 ||
                !this.IsLoaded)
            {
                return;
            }


            Array.Resize(ref this._coordinateCache, this.ItemsSource.Count);

            for (int i = 0; i < this.ItemsSource.Count; i++)
            {
                var item = this.ItemsSource[i];

                var pt = GetPlotCoordinateForItem(item);

                this._coordinateCache[i] = pt;

                var fe = this.PART_ScatterItemsControl?.TryGetElementForItem(item);

                if (fe != null)
                {
                    var scatter = (IScatter)fe;
                    scatter.Coordinate = pt;
                }
            }
            UpdateLineOrArea();
        }

        /// <summary>
        /// cache the coordinate for performance
        /// </summary>
        private PointNS[] _coordinateCache;
        internal PointNS[] GetCoordinates()
        {
            return this._coordinateCache;
        }
        #endregion


        /// <summary>
        /// Update the Data of path(line or area). This should be called after
        /// coordinates is calculated and <see cref="_coordinateCache"/> is updated
        /// </summary>
        protected abstract void UpdateLineOrArea();




    }



}
