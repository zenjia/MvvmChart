using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using MvvmChart.Common;

namespace MvvmCharting
{

    /// <summary>
    /// The base class for all series.
    /// </summary>
    [TemplatePart(Name = "PART_DataPointItemsControl", Type = typeof(SlimItemsControl))]
    [TemplatePart(Name = "PART_Path", Type = typeof(Path))]
    public abstract class SeriesBase : Control
    {

        private static readonly string sPART_Path = "PART_Path";
        private static readonly string sPART_ScatterItemsControl = "PART_DataPointItemsControl";
        private SlimItemsControl PART_ScatterItemsControl;
        protected Path PART_Path { get; private set; }

        public SeriesBase()
        {
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Path = (Path)GetTemplateChild(sPART_Path);
            if (this.PART_Path != null)
            {
                this.PART_Path.Visibility = this.IsLineOrAreaVisible ? Visibility.Visible : Visibility.Collapsed;
                this.PART_Path.Stroke = this.Stroke;
                this.PART_Path.StrokeThickness = this.StrokeThickness;
                this.PART_Path.Fill = this.Fill;
            }


            if (this.PART_ScatterItemsControl != null)
            {
                this.PART_ScatterItemsControl.ItemTemplateContentLoaded -= ScatterItemsControlScatterGenerated;

            }

            this.PART_ScatterItemsControl = (SlimItemsControl)GetTemplateChild(sPART_ScatterItemsControl);
            if (this.PART_ScatterItemsControl != null)
            {
                this.PART_ScatterItemsControl.ItemTemplateContentLoaded += ScatterItemsControlScatterGenerated;

             
                this.PART_ScatterItemsControl.Visibility = this.IsScatterVisible ? Visibility.Visible : Visibility.Collapsed;
           

                this.PART_ScatterItemsControl.ItemsSource = this.ItemsSource;

                this.PART_ScatterItemsControl.ItemTemplateSelector = this.ScatterTemplateSelector;
                this.PART_ScatterItemsControl.ItemTemplate = this.ScatterTemplate;

                UpdateScattersPosition();
            }



        }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);


            UpdateScattersPosition();
        }


        public event Action<Range> XRangeChanged;
        public event Action<Range> YRangeChanged;


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

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(SeriesBase), new PropertyMetadata(null, OnItemsSourcePropertyChanged));



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
     
 
               
                //if (this.IsScatterVisible)
                //{
                //    UpdateScattersPosition();
                //}
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

        #region Path properties
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty =
            Shape.StrokeProperty.AddOwner(typeof(SeriesBase), new PropertyMetadata(OnStrokePropertyChanged));

        private static void OnStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesBase)d).OnStrokeChanged();
        }

        private void OnStrokeChanged()
        {
            if (this.PART_Path != null)
            {
                this.PART_Path.Stroke = this.Stroke;
            }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner(typeof(SeriesBase), new PropertyMetadata(1.0, OnStrokeThicknessPropertyChanged));

        private static void OnStrokeThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ((SeriesBase)d).OnStrokeThicknessChanged();
        }

        private void OnStrokeThicknessChanged()
        {
            if (this.PART_Path != null)
            {
                this.PART_Path.StrokeThickness = this.StrokeThickness;
            }


        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            Shape.FillProperty.AddOwner(typeof(SeriesBase), new PropertyMetadata(OnFillPropertyChanged));

        private static void OnFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesBase)d).OnFillChanged();
        }
        private void OnFillChanged()
        {
            if (this.PART_Path != null)
            {
                this.PART_Path.Fill = this.Fill;
            }
        }

        #endregion


        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SeriesBase c = (SeriesBase)d;

            c.OnItemsSourceChanged((IList)e.OldValue, (IList)e.NewValue);



        }

        protected virtual void OnItemsSourceChanged(IList oldValue, IList newValue)
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
                if (this._coordinateCache == null)
                {
                    this._coordinateCache = new Point[newValue.Count];
                }
                else
                {
                    Array.Resize(ref this._coordinateCache, newValue.Count);
                }

                HandleItemsSourceCollectionChange(null, newValue);

                if (newValue is INotifyCollectionChanged newItemsSource)
                {
                    WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                        .AddHandler(newItemsSource, "CollectionChanged", ItemsSource_CollectionChanged);
                }
            }

        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        private Range _yDataRange = Range.Empty;
        public Range YDataRange
        {
            get { return this._yDataRange; }
            private set
            {
                if (this._yDataRange != value)
                {
                    this._yDataRange = value;

                    this.YRangeChanged?.Invoke(this.YDataRange);

                }

            }
        }

        private Range _xDataRange = Range.Empty;
        public Range XDataRange
        {
            get { return this._xDataRange; }
            private set
            {
                if (this._xDataRange != value)
                {
                    this._xDataRange = value;
                    this.XRangeChanged?.Invoke(this.XDataRange);


                }
            }
        }

        private Range _plotingXDataRange = Range.Empty;
        public Range PlotingXDataRange
        {
            get { return this._plotingXDataRange; }
            set
            {
                if (this._plotingXDataRange != value)
                {
                    this._plotingXDataRange = value;

                    UpdateScattersPosition();
                }
            }
        }

        private Range _plotingYDataRange = Range.Empty;
        public Range PlotingYDataRange
        {
            get { return this._plotingYDataRange; }
            set
            {
                if (this._plotingYDataRange != value)
                {
                    this._plotingYDataRange = value;

                    UpdateScattersPosition();

                }
            }
        }

        private readonly Dictionary<object, Scatter> _scatterCache = new Dictionary<object, Scatter>();
   

        protected void HandleItemsSourceCollectionChange(IList oldValue, IList newValue)
        {
 

            UpdateSeriesRange();
            UpdateScattersPosition();
            UpdatePathData();


        }



        private Point GetPlotCoordinateForItem(object item)
        {
            var itemValuePoint = GetPointFromItem(item);
            var pt = new Point((itemValuePoint.X - this.PlotingXDataRange.Min) * this.xPixelPerUnit - 1,
                (itemValuePoint.Y - this.PlotingYDataRange.Min) * this.yPixelPerUnit - 1);

            return pt;
        }

        protected Point[] _coordinateCache;
        private double xPixelPerUnit;
        private double yPixelPerUnit;

        private void UpdatePixelPerUnit()
        {
            if (this.PlotingXDataRange.IsInvalid ||
                this.PlotingYDataRange.IsInvalid ||
                this.RenderSize.IsInvalid())
            {
                this.xPixelPerUnit = double.NaN;
                this.yPixelPerUnit = double.NaN;

                return;
            }

            var plotAreaSize = this.RenderSize;
            if (plotAreaSize.Width.IsNaNOrZero())
            {
                return;
            }

            var xRange = this.PlotingXDataRange;
            var yRange = this.PlotingYDataRange;
            if (xRange.IsInvalid || yRange.IsInvalid )
            {
                this.xPixelPerUnit = double.NaN;
                this.yPixelPerUnit = double.NaN;
                return;
            }

            this.xPixelPerUnit = plotAreaSize.Width / xRange.Span;
            this.yPixelPerUnit = plotAreaSize.Height / yRange.Span;

        }

        private void UpdateScattersPosition()
        {
            this.PART_ScatterItemsControl?.LoadAllItems();
            OnIsSeriesPointsVisibleChanged();
            UpdatePixelPerUnit();


            if (this.xPixelPerUnit.IsNaN() ||
                this.yPixelPerUnit.IsNaN() ||
                this.PART_ScatterItemsControl == null ||
                this.ItemsSource == null ||
                this.ItemsSource.Count == 0)
            {
             
                return;
            }


            for (int i = 0; i < this.ItemsSource.Count; i++)
            {
                var item = this.ItemsSource[i];

                var pt = GetPlotCoordinateForItem(item);

                this._coordinateCache[i] = pt;

                var fe = this.PART_ScatterItemsControl.TryGetTemplateElementForItem(item);
               
                if (fe != null)
                {
                    var scatter = (Scatter)fe;
                    scatter.Coordinate = pt;
                }


            }


            if (this.PART_ScatterItemsControl.ItemCount == 0)
            {
                this.PART_ScatterItemsControl.LoadAllItems();
              
            }
            UpdatePathData();
        }





        protected abstract void UpdatePathData();

        internal Point GetPointFromItem(object item)
        {
            var t = item.GetType();
            var x = t.GetProperty(this.IndependentValueProperty).GetValue(item);
            var y = t.GetProperty(this.DependentValueProperty).GetValue(item);

            var pt = new Point(DoubleValueConverter.ObjectToDouble(x), DoubleValueConverter.ObjectToDouble(y));
            return pt;

        }

        internal void UpdateSeriesRange()
        {
            if (this.ItemsSource == null ||
                this.ItemsSource.Count == 0)
            {
                this.XDataRange = Range.Empty;
                this.YDataRange = Range.Empty;

                return;
            }


            double minX = double.MaxValue, minY = double.MaxValue,
                maxX = double.MinValue, maxY = double.MinValue;

            foreach (var item in this.ItemsSource)
            {
                var pt = GetPointFromItem(item);

                var x = pt.X;
                var y = pt.Y;


                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);

                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);

            }

            this.XDataRange = new Range(minX, maxX);
            this.YDataRange = new Range(minY, maxY);
        }




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


        private void ScatterItemsControlScatterGenerated(object sender, DependencyObject root)
        {
            var scatter = (Scatter)root;
            if (scatter == null)
            {
                throw new MvvmChartException("The root element in the ScatterTemplate should be of type: 'Scatter'.");
            }

            var item = scatter.DataContext;
           // this._scatterCache.Add(item, scatter);

            if (!this.xPixelPerUnit.IsNaN() && !this.yPixelPerUnit.IsNaN())
            {
                scatter.Coordinate = GetPlotCoordinateForItem(item);

            }




        }
    }



}
