using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MvvmCharting.Common;
using MvvmCharting.Series;
using Range = MvvmCharting.Common.Range;


namespace MvvmCharting.WpfFX.Series
{
    /// <summary>
    /// Represents a series control which can hold as many as four types of series:
    /// <see cref="LineSeries"/>, <see cref="AreaSeries"/>, <see cref="ScatterSeries"/>,
    /// <see cref="BarSeries"/>. They can be added, removed or hid dynamically.
    /// When overlapped one another, they together can compose a sophisticated series
    /// as a whole. 
    /// </summary>
    public class SeriesControl : InteractiveControl, ISeriesControl,
        IScatterSeriesOwner,
        ILineSeriesOwner,
        IBarSeriesOwner
    {
        private static readonly IValueConverter B2v = new BooleanToVisibilityConverter();
        static SeriesControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SeriesControl), new FrameworkPropertyMetadata(typeof(SeriesControl)));
        }

        public event Action<ISeriesControl, Range> XRangeChanged;
        public event Action<ISeriesControl, Range> YRangeChanged;

        public event Action<ISeriesControl, Range2D> ValueRangeChanged;
        public event Action<ISeriesControl, Range2D> RawValueRangeChanged;

        public ISeriesControlOwner SeriesControlOwner => this.Owner;

        private ISeriesControlOwner _owner;
        internal ISeriesControlOwner Owner
        {
            get => _owner;
            set
            {
                if (_owner != value)
                {
                    this._owner = value;

                    //this.LineSeries?.UpdateShape();
                    //this.AreaSeries?.UpdateShape();
                    //this.ScatterSeries?.UpdateCoordinate();
                    //this.BarSeries?.UpdateBarWidth(this.MinXValueGap, this.XPixelPerUnit);
                    //this.BarSeries?.UpdateBarItemCoordinateAndHeight();
                }

            }
        }

        private ContentControl PART_LineSeriesHolder;
        private ContentControl PART_AreaSeriesHolder;
        private ContentControl PART_ScatterSeriesHolder;
        private ContentControl PART_BarSeriesHolder;

        public SeriesControl()
        {
            this.Loaded += SeriesHost_Loaded;

        }

        private void SeriesHost_Loaded(object sender, RoutedEventArgs e)
        {
            RecalculateCoordinate();
        }

        #region overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_LineSeriesHolder = (ContentControl)GetTemplateChild("PART_LineSeriesHolder");
            if (this.PART_LineSeriesHolder != null)
            {
                this.PART_LineSeriesHolder.Content = this.LineSeries;
                this.PART_LineSeriesHolder.SetBinding(VisibilityProperty,
                    new Binding(nameof(this.IsLineSeriesVisible)) { Source = this, Converter = B2v });
            }

            this.PART_AreaSeriesHolder = (ContentControl)GetTemplateChild("PART_AreaSeriesHolder");
            if (this.PART_AreaSeriesHolder != null)
            {
                this.PART_AreaSeriesHolder.Content = this.AreaSeries;
                this.PART_AreaSeriesHolder.SetBinding(ContentControl.VisibilityProperty,
                   new Binding(nameof(this.IsAreaSeriesVisible)) { Source = this, Converter = B2v });
            }

            this.PART_ScatterSeriesHolder = (ContentControl)GetTemplateChild("PART_ScatterSeriesHolder");
            if (this.PART_ScatterSeriesHolder != null)
            {
                this.PART_ScatterSeriesHolder.Content = this.ScatterSeries;
                this.PART_ScatterSeriesHolder.SetBinding(VisibilityProperty,
                    new Binding(nameof(this.IsScatterSeriesVisible)) { Source = this, Converter = B2v });
            }

            this.PART_BarSeriesHolder = (ContentControl)GetTemplateChild("PART_BarSeriesHolder");
            if (this.PART_BarSeriesHolder != null)
            {
                this.PART_BarSeriesHolder.Content = this.BarSeries;
                this.PART_BarSeriesHolder.SetBinding(VisibilityProperty,
                    new Binding(nameof(this.IsBarSeriesVisible)) { Source = this, Converter = B2v });
            }
        }




        #endregion

        #region Validate data


        internal void ValidateData()
        {


            if (!this.Owner.IsXAxisCategory && (this.Owner.StackMode == StackMode.NotStacked))
            {
                return;
            }

            var firstSh = this.Owner.GetSeries().FirstOrDefault(x => x != this && x.ItemsSource != null);

            if (firstSh == null)
            {
                return;
            }

            string strReason = this.Owner.StackMode != StackMode.NotStacked ? $"In {this.Owner.StackMode} mode" : "If the XAxis of a Chart is CategoryAxis";

            if (firstSh.ItemsSource.Count != this.ItemsSource.Count)
            {
                throw new MvvmChartModelDataException($"{strReason}, the item count of all series in a Chart must be same!");
            }

            for (int i = 0; i < this.ItemsSource.Count; i++)
            {
                if (this.Owner.IsXAxisCategory)
                {
                    var x1 = GetXPropertyObjectForItem(this.ItemsSource[i]);
                    var x2 = GetXPropertyObjectForItem(firstSh.ItemsSource[i]);
                    if (x1 == null || x2 == null)
                    {
                        throw new MvvmChartModelDataException("An Item's X value of the series ItemsSource cannot be null!");
                    }

                    if (!x1.Equals(x2))
                    {
                        throw new MvvmChartModelDataException($"{strReason}, the item's x value in the same index of all series in a Chart must be same!");
                    }
                }
                else
                {
                    var x1 = GetXValueForItem(this.ItemsSource[i]);
                    var x2 = GetXValueForItem(firstSh.ItemsSource[i]);

                    if (!x1.NearlyEqual(x2))
                    {
                        throw new MvvmChartModelDataException($"{strReason}, the item's x value in the same index of all series in a Chart must be same!");
                    }
                }

            }



        }

        #endregion

        #region IndependentValueProperty & DependentValueProperty properties
        public string IndependentValueProperty
        {
            get { return (string)GetValue(IndependentValuePropertyProperty); }
            set { SetValue(IndependentValuePropertyProperty, value); }
        }
        public static readonly DependencyProperty IndependentValuePropertyProperty =
            DependencyProperty.Register("IndependentValueProperty", typeof(string), typeof(SeriesControl), new PropertyMetadata(null));


        public string DependentValueProperty
        {
            get { return (string)GetValue(DependentValuePropertyProperty); }
            set { SetValue(DependentValuePropertyProperty, value); }
        }
        public static readonly DependencyProperty DependentValuePropertyProperty =
            DependencyProperty.Register("DependentValueProperty", typeof(string), typeof(SeriesControl), new PropertyMetadata(null));
        #endregion

        #region IsScatterSeriesVisible & IsLineSeriesVisible & IsAreaSeriesVisible properties
        public bool IsScatterSeriesVisible
        {
            get { return (bool)GetValue(IsScatterSeriesVisibleProperty); }
            set { SetValue(IsScatterSeriesVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsScatterSeriesVisibleProperty =
            DependencyProperty.Register("IsScatterSeriesVisible", typeof(bool), typeof(SeriesControl), new PropertyMetadata(true));

        public bool IsLineSeriesVisible
        {
            get { return (bool)GetValue(IsLineSeriesVisibleProperty); }
            set { SetValue(IsLineSeriesVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsLineSeriesVisibleProperty =
            DependencyProperty.Register("IsLineSeriesVisible", typeof(bool), typeof(SeriesControl), new PropertyMetadata(true));

        public bool IsAreaSeriesVisible
        {
            get { return (bool)GetValue(IsAreaSeriesVisibleProperty); }
            set { SetValue(IsAreaSeriesVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsAreaSeriesVisibleProperty =
            DependencyProperty.Register("IsAreaSeriesVisible", typeof(bool), typeof(SeriesControl), new PropertyMetadata(true));

 

        public bool IsBarSeriesVisible
        {
            get { return (bool)GetValue(IsBarSeriesVisibleProperty); }
            set { SetValue(IsBarSeriesVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsBarSeriesVisibleProperty =
            DependencyProperty.Register("IsBarSeriesVisible", typeof(bool), typeof(SeriesControl), new PropertyMetadata(true));



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
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(SeriesControl), new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            SeriesControl c = (SeriesControl)d;


            c.OnItemsSourceChanged((IList)e.OldValue, (IList)e.NewValue);

        }


        private void DetachCollectionChangedHandler(IList list)
        {
            if (list is INotifyCollectionChanged collection)
            {
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .RemoveHandler(collection, "CollectionChanged", ItemsSource_CollectionChanged);
            }
        }

        private void AttachCollectionChangedHandler(IList list)
        {
            if (list is INotifyCollectionChanged collection)
            {
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .AddHandler(collection, "CollectionChanged", ItemsSource_CollectionChanged);
            }
        }


        private void OnItemsSourceChanged(IList oldValue, IList newValue)
        {
            ValidateData();

            DetachCollectionChangedHandler(oldValue);
            AttachCollectionChangedHandler(newValue);

            Reset();

            //todo: performance!!
            this.ScatterSeries?.UpdateItemsSource();
            this.BarSeries?.UpdateItemsSource();

            UpdateRawValueRange(newValue);

            if (this.Owner.StackMode == StackMode.NotStacked ||
                this.Owner.GetSeries().All(sr => sr.ItemsSource != null))
            {
                if (!this.Owner.IsSeriesCollectionChanging)
                {
                    UpdateValueRange();
                    Refresh();
                }
            }




        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                return;
            }

            if (!this.Owner.IsSeriesCollectionChanging)
            {
                if (this.Owner.StackMode != StackMode.NotStacked)
                {
                    throw new MvvmChartModelDataException($"Collection change of ItemsSource is not allowed in {this.Owner.StackMode} mode when {nameof(this.Owner.IsSeriesCollectionChanging)} is false!");
                }
            }

            OnCollectionChanged(e);
            //HandleItemsSourceCollectionChange(e.OldItems, e.NewItems);
        }



        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null && args.OldItems.Count != 1 ||
                args.NewItems != null && args.NewItems.Count != 1)
            {
                throw new NotSupportedException(MvvmChartException.RangeActionsNotSupported);
            }

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnItemAdded(args.NewItems[0], args.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    OnItemRemoved(args.OldItems[0], args.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    OnItemReplaced(args.OldItems[0], args.NewItems[0], args.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Move:
                    OnItemMoved();
                    break;

                case NotifyCollectionChangedAction.Reset:
                    ResetValueRange();
                    this.RawValueRange = Range2D.Empty;
                    break;

                default:
                    throw new NotSupportedException($"{MvvmChartException.UnexpectedCollectionChangeAction}: {args.Action}");
            }

        }

        private void OnItemAdded(object newItem, int index)
        {


            //if (this.Owner.StackMode == StackMode.NotStacked)
            //{
            //    TryUpdateRawValueRangeOnItemAdd(newItem);
            //}

            if (!this.Owner.IsSeriesCollectionChanging)
            {
                UpdateValueRange();
                Refresh();
            }

        }

        private void OnItemRemoved(object oldItem, int index)
        {
            //if (this.Owner.StackMode != StackMode.NotStacked ||
            //    !this.RawValueRange.IsEmpty)
            //{
            //    TryResetRawValueRangeOnItemRemove(oldItem);
            //}

            if (!this.Owner.IsSeriesCollectionChanging)
            {
                UpdateValueRange();
                Refresh();
            }

        }

        private void OnItemReplaced(object oldItem, object newItem, int index)
        {


            //if (this.Owner.StackMode == StackMode.NotStacked)
            //{
            //    TryResetRawValueRangeOnItemRemove(oldItem);
            //    if (!this.RawValueRange.IsEmpty)
            //    {
            //        TryUpdateRawValueRangeOnItemAdd(newItem);
            //    }
            //}

            if (!this.Owner.IsSeriesCollectionChanging)
            {
                UpdateValueRange();
                Refresh();
            }

        }

        private void OnItemMoved()
        {
            throw new NotSupportedException();
        }

        private double _minXValueGap = double.NaN;
        public double MinXValueGap
        {
            get { return this._minXValueGap; }
            set
            {
                if (this._minXValueGap != value)
                {
                    this._minXValueGap = value;
                    if (!value.IsNaN())
                    {
                        this.BarSeries?.UpdateBarWidth(this.MinXValueGap, this.XPixelPerUnit);
                    }
                }
            }
        }
        private void UpdateMinXValueGap()
        {
            if (this.Owner == null || this.BarSeries == null)
            {
                return;
            }

            if (this.Owner.IsXAxisCategory)
            {
                if (this.MinXValueGap != 1.0)
                {
                    this.MinXValueGap = 1.0;
                }
                else
                {
                    this.BarSeries?.UpdateBarWidth(this.MinXValueGap, this.XPixelPerUnit);
                }

                return;
            }

            if (this.ItemsSource == null || this.ItemsSource.Count == 0)
            {
                return;
            }

            double prev = GetXValueForItem(this.ItemsSource[0]);
            double minGap = double.MaxValue;
            for (var i = 1; i < this.ItemsSource.Count; i++)
            {
                var item = this.ItemsSource[i];

                var current = GetXValueForItem(item);
                var gap = current - prev;
                if (gap < minGap)
                {
                    minGap = gap;
                }

                prev = current;
            }

            this.MinXValueGap = minGap;
        }



        private double GetXValueForItem(object item)
        {
            var t = item.GetType();
            var x = t.GetProperty(this.IndependentValueProperty).GetValue(item);

            return DoubleValueConverter.ObjectToDouble(x);

        }

        internal object GetXPropertyObjectForItem(object item)
        {
            var t = item.GetType();
            var x = t.GetProperty(this.IndependentValueProperty).GetValue(item);

            return x;

        }

        private double GetYValueForItem(object item)
        {
            var t = item.GetType();
            var yProp = t.GetProperty(this.DependentValueProperty);
            var y = yProp.GetValue(item);

            return DoubleValueConverter.ObjectToDouble(y);
        }

        private double GetAdjustYValueForItem(object item, int index)
        {
            double minY = this.Owner.YBaseValue;

            switch (this.Owner.StackMode)
            {
                case StackMode.NotStacked:
                    return GetYValueForItem(item)/* - minY*/;

                case StackMode.Stacked:
                    double total = 0;
                    foreach (var sr in this.Owner.GetSeries())
                    {
                        var obj = sr.ItemsSource[index];
                        var y = sr.GetYValueForItem(obj) - minY;
                        total += y;
                        if (obj == item)
                        {
                            break;
                        }
                    }

                    return total;

                case StackMode.Stacked100:
                    bool meet = false;
                    total = 0;
                    double sum = 0;
                    foreach (var sr in this.Owner.GetSeries())
                    {
                        var obj = sr.ItemsSource[index];
                        total += sr.GetYValueForItem(obj) - minY;

                        if (!meet)
                        {
                            sum = total;
                        }

                        if (obj == item)
                        {
                            meet = true;
                        }

                    }

                    return sum / total;
                default:
                    throw new ArgumentOutOfRangeException();
            }




        }

        private void ResetValueRange()
        {
            this.ValueRange = Range2D.Empty;

        }

        public void UpdateValueRange()
        {

            if (this.ItemsSource == null ||
                this.ItemsSource.Count == 0)
            {
                ResetValueRange();
                return;
            }

            double minY = double.MaxValue;
            double maxY = double.MinValue;
            double minX = double.MaxValue;
            double maxX = double.MinValue;

            for (int i = 0; i < this.ItemsSource.Count; i++)
            {
                var item = this.ItemsSource[i];

                var y = GetAdjustYValueForItem(item, i);

                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);



                if (!this.Owner.IsXAxisCategory)
                {
                    var x = GetXValueForItem(item);

                    minX = Math.Min(minX, x);
                    maxX = Math.Max(maxX, x);
                }
            }


            if (this.Owner.IsXAxisCategory)
            {
                minX = 0;
                maxX = this.ItemsSource.Count;
            }
            this.ValueRange = new Range2D(minX, maxX, minY, maxY);


        }

        internal void UpdateRawValueRange(IList source)
        {
            if (this.Owner.StackMode == StackMode.NotStacked)
            {
                return;
            }

            if (source == null || source.Count == 0)
            {
                return;
            }

            double minY = double.MaxValue;
            double maxY = double.MinValue;

            for (int i = 0; i < source.Count; i++)
            {
                var item = source[i];

                var y = GetYValueForItem(item);

                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);

            }

            double minX;
            double maxX;
            if (this.Owner.IsXAxisCategory)
            {
                minX = 0;
                maxX = this.ItemsSource.Count;
            }
            else
            {
                minX = GetXValueForItem(source[0]);
                maxX = GetXValueForItem(source[source.Count - 1]);
            }

            if (this.RawValueRange.IsOutOfRange(minX, maxX, minY, maxY))
            {
                this.RawValueRange = new Range2D(minX, maxX, minY, maxY);
            }
        }

        #endregion

        #region value range

        private Range2D _valueRange;
        public Range2D ValueRange
        {
            get { return this._valueRange; }
            set
            {
                if (!this._valueRange.Equals(value))
                {
                    this._valueRange = value;
                    this.ValueRangeChanged?.Invoke(this, value);
                }

            }
        }

        private Range2D _rawValueRange;
        public Range2D RawValueRange
        {
            get { return this._rawValueRange; }
            set
            {
                if (!this._rawValueRange.Equals(value))
                {
                    this._rawValueRange = value;
                    this.RawValueRangeChanged?.Invoke(this, value);
                }

            }
        }
        #endregion

        #region Coordinates calculating

        public double XPixelPerUnit => this.Owner.XPixelPerUnit;

        public double YPixelPerUnit => this.Owner.YPixelPerUnit;

        public Point GetPlotCoordinateForItem(object item, int itemIndex)
        {
            double x;

            if (!this.Owner.IsXAxisCategory)
            {
                x = GetXValueForItem(item);

            }
            else
            {
                x = itemIndex + 0.5;
            }

            var y = GetAdjustYValueForItem(item, itemIndex);

            var pt = new Point((x - this.Owner.XStartValue) * this.XPixelPerUnit,
                (y/* + this.Owner.YBaseValue*/ - this.Owner.YStartValue) * this.YPixelPerUnit);

            return pt;
        }

        /// <summary>
        /// This should be call when: 1) ItemsSource; 2) x or y PixelPerUnit, and 3) RenderSize changed.
        /// This method will first update the _coordinateCache and the Coordinate of each scatter,
        /// then update the shape of the Line or Area
        /// </summary>

        internal void RecalculateCoordinate()
        {
            if (this.Owner.IsSeriesCollectionChanging)
            {
                return;
            }

            if (this.XPixelPerUnit.IsNaN() ||
                this.YPixelPerUnit.IsNaN() ||
                !this.IsLoaded)
            {
                return;
            }

            Array.Resize(ref this._coordinateCache, this.ItemsSource.Count);
            var previous = GetPreviousCoordinates(false);

            var prevX = double.NaN;
            double minGap = double.MaxValue;
            for (int i = 0; i < this.ItemsSource.Count; i++)
            {
                var item = this.ItemsSource[i];

                var pt = GetPlotCoordinateForItem(item, i);

                this._coordinateCache[i] = pt;

                this.ScatterSeries?.UpdateScatterCoordinate(item, pt);

                double y = previous?[i].Y ?? this.Owner.YBaseCoordinate;
                this.BarSeries?.UpdateBarCoordinateAndHeight(item, pt, y);

                if (!this.Owner.IsXAxisCategory)
                {
                    if (!prevX.IsNaN())
                    {
                        double gap = pt.X = prevX;
                        if (gap < minGap)
                        {
                            minGap = gap;
                        }
                    }

                    prevX = pt.X;
                }

            }

            this.LineSeries?.UpdateShape();
            this.AreaSeries?.UpdateShape();

            this.MinXValueGap = this.Owner.IsXAxisCategory ? 1.0 : minGap;


            Debug.WriteLine($"{this.DataContext}...RecalculateCoordinate...ok!!!");
        }

        internal void Reset()
        {

            ResetValueRange();
        }

        internal void Refresh()
        {

            RecalculateCoordinate();

        }

        /// <summary>
        /// cache the coordinate for performance
        /// </summary>
        private Point[] _coordinateCache;

        public Point[] GetCoordinates()
        {
            return this._coordinateCache;
        }

        private ISeriesControl GetPreviousSeriesHost()
        {
            return this.Owner.GetPreviousSeriesHost(this);
        }

        //public double StartCoordinateY => (this.PlottingYValueRange.ActualRange.Min -
        //                              this.PlottingYValueRange.FullRange.Min) * this.YPixelPerUnit;

        public Point[] GetPreviousCoordinates(bool isAreaSeries)
        {
            var coordinates = GetCoordinates();

            Point[] previous = null;

            double yBaseCoordinate = this.Owner.YBaseCoordinate;

            switch (this.Owner.StackMode)
            {
                case StackMode.NotStacked:
                    if (isAreaSeries)
                    {

                        previous = new[] { new Point(coordinates.First().X, yBaseCoordinate), new Point(coordinates.Last().X, yBaseCoordinate) };
                    }
                    break;
                case StackMode.Stacked:
                case StackMode.Stacked100:
                    var previousSeriesControl = (SeriesControl)GetPreviousSeriesHost();
                    if (previousSeriesControl == null)
                    {
                        if (isAreaSeries)
                        {
                            previous = new[] { new Point(coordinates.First().X, yBaseCoordinate), new Point(coordinates.Last().X, yBaseCoordinate) };
                        }

                    }
                    else
                    {
                        previous = previousSeriesControl.GetCoordinates();


                        if (previous.Length != coordinates.Length)
                        {
                            throw new MvvmChartException($"{this.DataContext}...previous.Length({previous.Length}) != coordinates.Length({coordinates.Length})");
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return previous;
        }

        #endregion

        #region series
        public LineSeries LineSeries
        {
            get { return (LineSeries)GetValue(LineSeriesProperty); }
            set { SetValue(LineSeriesProperty, value); }
        }
        public static readonly DependencyProperty LineSeriesProperty =
            DependencyProperty.Register("LineSeries", typeof(LineSeries), typeof(SeriesControl), new PropertyMetadata(null, OnLineSeriesPropertyChanged));

        private static void OnLineSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesControl)d).OnLineSeriesChanged();
        }

        private void OnLineSeriesChanged()
        {
            if (this.PART_LineSeriesHolder != null)
            {
                this.PART_LineSeriesHolder.Content = this.LineSeries;
            }

            if (this.LineSeries != null)
            {
                this.LineSeries.Owner = this;
                this.LineSeries.UpdateShape();
            }

        }

        public AreaSeries AreaSeries
        {
            get { return (AreaSeries)GetValue(AreaSeriesProperty); }
            set { SetValue(AreaSeriesProperty, value); }
        }
        public static readonly DependencyProperty AreaSeriesProperty =
            DependencyProperty.Register("AreaSeries", typeof(AreaSeries), typeof(SeriesControl), new PropertyMetadata(null, OnAreaSeriesPropertyChanged));

        private static void OnAreaSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesControl)d).OnAreaSeriesChanged();
        }

        private void OnAreaSeriesChanged()
        {
            if (this.PART_AreaSeriesHolder != null)
            {
                this.PART_AreaSeriesHolder.Content = this.AreaSeries;
            }

            if (this.AreaSeries != null)
            {
                this.AreaSeries.Owner = this;
                this.AreaSeries.UpdateShape();
            }
        }

        public ScatterSeries ScatterSeries
        {
            get { return (ScatterSeries)GetValue(ScatterSeriesProperty); }
            set { SetValue(ScatterSeriesProperty, value); }
        }
        public static readonly DependencyProperty ScatterSeriesProperty =
            DependencyProperty.Register("ScatterSeries", typeof(ScatterSeries), typeof(SeriesControl), new PropertyMetadata(null, OnScatterSeriesPropertyChanged));

        private static void OnScatterSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesControl)d).OnScatterSeriesChanged();
        }

        private void OnScatterSeriesChanged()
        {
            if (this.PART_ScatterSeriesHolder != null)
            {
                this.PART_ScatterSeriesHolder.Content = this.ScatterSeries;
            }

            if (this.ScatterSeries != null)
            {
                this.ScatterSeries.Owner = this;
                this.ScatterSeries.UpdateItemsSource();
            }
        }


        public BarSeries BarSeries
        {
            get { return (BarSeries)GetValue(BarSeriesProperty); }
            set { SetValue(BarSeriesProperty, value); }
        }
        public static readonly DependencyProperty BarSeriesProperty =
            DependencyProperty.Register("BarSeries", typeof(BarSeries), typeof(SeriesControl), new PropertyMetadata(null, OnBarSeriesPropertyChanged));

        private static void OnBarSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesControl)d).OnBarSeriesChanged();
        }
        private void OnBarSeriesChanged()
        {
            if (this.PART_BarSeriesHolder != null)
            {

                this.PART_BarSeriesHolder.Content = this.BarSeries;
            }

            if (this.BarSeries != null)
            {

                this.BarSeries.Owner = this;
                this.BarSeries.UpdateItemsSource();
            }
        }
        #endregion


    }



}
