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
        private static readonly string sPART_DataPointItemsControl = "PART_DataPointItemsControl";
        private SlimItemsControl PART_ItemsControl;
        protected Path PART_Path { get; private set; }

        public SeriesBase()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Path = (Path)this.GetTemplateChild(sPART_Path);
            if (this.PART_Path != null)
            {
                this.PART_Path.Visibility = this.IsLineOrAreaVisible ? Visibility.Visible : Visibility.Collapsed;
                this.PART_Path.Stroke = this.Stroke;
                this.PART_Path.StrokeThickness = this.StrokeThickness;
                this.PART_Path.Fill = this.Fill;
            }


            if (this.PART_ItemsControl != null)
            {
                this.PART_ItemsControl.ItemTemplateContentLoaded -= PART_ItemsControl_ItemPointGenerated;
            }

            this.PART_ItemsControl = (SlimItemsControl)GetTemplateChild(sPART_DataPointItemsControl);
            if (this.PART_ItemsControl != null)
            {
                this.PART_ItemsControl.ItemTemplateContentLoaded += PART_ItemsControl_ItemPointGenerated;
                this.PART_ItemsControl.Visibility = this.IsScatterVisible ? Visibility.Visible : Visibility.Collapsed;

                this.PART_ItemsControl.ItemsSource = this._dataPointViewModels;
                this.PART_ItemsControl.ItemTemplateSelector = this.ScatterTemplateSelector;
                this.PART_ItemsControl.ItemTemplate = this.ScatterTemplate;
            }



        }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdatePointsPosition();
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
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(SeriesBase), new PropertyMetadata(null, OnPointItemsSourcePropertyChanged));



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
            if (this.PART_ItemsControl != null)
            {
                this.PART_ItemsControl.Visibility = this.IsScatterVisible ? Visibility.Visible : Visibility.Collapsed;
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
            if (this.PART_ItemsControl != null)
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


        private static void OnPointItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SeriesBase c = (SeriesBase)d;

            c.OnPointItemsSourcePropertyChanged((IList)e.OldValue, (IList)e.NewValue);



        }

        protected virtual void OnPointItemsSourcePropertyChanged(IList oldValue, IList newValue)
        {

            LoadItemPointViewModels(null, this.ItemsSource);

            if (oldValue is INotifyCollectionChanged oldItemsSource)
            {
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .AddHandler(oldItemsSource, "CollectionChanged", SeriesBase_CollectionChanged);
            }

            if (newValue is INotifyCollectionChanged newItemsSource)
            {
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .AddHandler(newItemsSource, "CollectionChanged", SeriesBase_CollectionChanged);
            }
        }

        private void SeriesBase_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this._dataPointViewModels?.Clear();
            }

            LoadItemPointViewModels(e.OldItems, e.NewItems);
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

        private Range _plotAreaXDataRange = Range.Empty;
        public Range PlotAreaXDataRange
        {
            get { return this._plotAreaXDataRange; }
            set
            {
                if (this._plotAreaXDataRange != value)
                {
                    this._plotAreaXDataRange = value;



                    UpdatePointsPosition();
                }
            }
        }

        private Range _plotAreaYDataRange = Range.Empty;
        public Range PlotAreaYDataRange
        {
            get { return this._plotAreaYDataRange; }
            set
            {
                if (this._plotAreaYDataRange != value)
                {
                    this._plotAreaYDataRange = value;


                    UpdatePointsPosition();

                }
            }
        }

        protected readonly ObservableCollection<ScatterViewModel> _dataPointViewModels = new ObservableCollection<ScatterViewModel>();


        protected void LoadItemPointViewModels(IList oldValue, IList newValue)
        {
            if (oldValue != null)
            {
                foreach (var item in oldValue)
                {
                    this._dataPointViewModels.Remove(x => x.Item == item);
                }
            }

            if (newValue != null)
            {
                foreach (var item in newValue)
                {
                    var dpvm = new ScatterViewModel(item);
                    this._dataPointViewModels.Add(dpvm);
                }
            }

          
            UpdateSeriesRange();
            UpdatePointsPosition();
            UpdateShape();

   
        }

        private void UpdatePointsPosition()
        {
            if (this.PlotAreaXDataRange.IsEmpty ||
                this.PlotAreaYDataRange.IsEmpty ||
                this.RenderSize.IsInvalid() ||
                this._dataPointViewModels.Count == 0)
            {
                return;
            }

            var plotAreaSize = this.RenderSize;

            var xRange = this.PlotAreaXDataRange;
            var yRange = this.PlotAreaYDataRange;
            double xUnit = plotAreaSize.Width / xRange.Span;
            double yUnit = plotAreaSize.Height / yRange.Span;
            foreach (var dpvm in this._dataPointViewModels)
            {
                var pt = GetPointFromItem(dpvm.Item);
                dpvm.Position = new Point((pt.X - this.PlotAreaXDataRange.Min) * xUnit - 1, (pt.Y - this.PlotAreaYDataRange.Min) * yUnit - 1);
               

            }


            UpdateShape();
        }





        protected abstract void UpdateShape();







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
            if (this._dataPointViewModels.Count == 0)
            {
                this.XDataRange = Range.Empty;
                this.YDataRange = Range.Empty;

                return;
            }


            double minX = double.MaxValue, minY = double.MaxValue,
                maxX = double.MinValue, maxY = double.MinValue;

            foreach (var item in this._dataPointViewModels)
            {
                var pt = GetPointFromItem(item.Item);

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
            if (this.PART_ItemsControl != null)
            {
                this.PART_ItemsControl.ItemTemplate = this.ScatterTemplate;
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
            if (this.PART_ItemsControl != null)
            {
                this.PART_ItemsControl.ItemTemplateSelector = this.ScatterTemplateSelector;
            }
        }


        private void PART_ItemsControl_ItemPointGenerated(object arg1, DependencyObject root)
        {
            var scatter = VisualTreeHelper2.GetAllChildren(root).OfType<Scatter>().SingleOrDefault();
            if (scatter == null)
            {
                throw new Cartesian2DChartException("The root element in the ScatterTemplate should be of Scatter type!");
            }


            Binding b = new Binding();
            b.Path = new PropertyPath(nameof(ScatterViewModel.Position));
            scatter.SetBinding(Scatter.PositionProperty, b);



        }
    }



}
