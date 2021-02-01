using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MvvmCharting.Common;
using MvvmCharting.Series;
using Range = MvvmCharting.Common.Range;

namespace MvvmCharting.WpfFX.Series
{
    public interface IChart
    {
        double XMinimum { get; }
        double YMinimum { get; }

        double XMaximum { get; }
        double YMaximum { get; }

        Point XValuePadding { get; }
        Point YValuePadding { get; }

        IList SeriesItemsSource { get; }
        DataTemplate SeriesTemplate { get; }

        DataTemplateSelector SeriesTemplateSelector { get; }

        StackMode SeriesStackMode { get; }

        bool IsSeriesCollectionChanging { get; }

        double YBaseValue { get; }

    }

    /// <summary>
    /// Used to hold a collection of series
    /// </summary>
    [TemplatePart(Name = "PART_SeriesItemsControl", Type = typeof(SlimItemsControl))]
    public class SeriesCollectionControl : Control, ISeriesControlOwner
    {
        private static readonly string sPART_SeriesItemsControl = "PART_SeriesItemsControl";
        static SeriesCollectionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SeriesCollectionControl), new FrameworkPropertyMetadata(typeof(SeriesCollectionControl)));
        }

        internal event Action<Range> ActualXPlottingRangeChanged;
        internal event Action<Range> ActualYPlottingRangeChanged;

        private IChart _owner;
        internal IChart Owner
        {
            get { return this._owner; }
            set
            {
                if (this._owner != value)
                {
                    this._owner = value;
                    OnOwnerChanged();
                }

            }
        }

        private void OnOwnerChanged()
        {
            this.IsSeriesCollectionChanging = this.Owner.IsSeriesCollectionChanging;
            OnSeriesTemplateChanged();
            OnSeriesTemplateSelectorChanged();
            OnSeriesItemsSourceChanged(null, this.Owner.SeriesItemsSource);
        }

        private SlimItemsControl PART_SeriesItemsControl;

        private bool _isXAxisCategory;
        public bool IsXAxisCategory
        {
            get { return this._isXAxisCategory; }
            internal set
            {
                if (this._isXAxisCategory != value)
                {
                    this._isXAxisCategory = value;

                    if (value)
                    {
                        ValidateData();

                    }
                }
            }
        }

        private bool _isSeriesCollectionChanging;
        public bool IsSeriesCollectionChanging
        {
            get { return this._isSeriesCollectionChanging; }
            internal set
            {
                if (this.IsSeriesCollectionChanging && !value)
                {
                    UpdateValueRange();
                }

                if (this._isSeriesCollectionChanging != value)
                {
                    this._isSeriesCollectionChanging = value;
                    if (!value)
                    {
                        Refresh();
                    }

                }
            }
        }

        private int GetSeriesIndex(ISeriesControl seriesControl)
        {
            var item = seriesControl.DataContext;
            if (this.itemIndexMap.TryGetValue(item, out int index))
            {
                return index;
            }

            return -1;
        }

        private ISeriesControl GetSeriesHost(int index)
        {
            var item = this.SeriesItemsSource[index];
            return (ISeriesControl)this.PART_SeriesItemsControl?.TryGetChildForItem(item);
        }

        public ISeriesControl GetPreviousSeriesHost(ISeriesControl current)
        {
            int index = GetSeriesIndex(current);
            if (index <= 0)
            {
                return null;
            }

            return GetSeriesHost(index - 1);
        }

        private int SeriesCount => this.PART_SeriesItemsControl?.ItemCount ?? 0;


        public IEnumerable<SeriesControl> GetSeries()
        {
            if (this.PART_SeriesItemsControl == null)
            {
                return Enumerable.Empty<SeriesControl>();
            }

            return this.PART_SeriesItemsControl.GetChildren().OfType<SeriesControl>();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.PART_SeriesItemsControl != null)
            {
                this.PART_SeriesItemsControl.ElementGenerated -= SeriesItemTemplateApplied;
            }

            this.PART_SeriesItemsControl = (SlimItemsControl)GetTemplateChild(sPART_SeriesItemsControl);

            if (this.PART_SeriesItemsControl != null)
            {
                this.PART_SeriesItemsControl.ElementGenerated += SeriesItemTemplateApplied;
                this.PART_SeriesItemsControl.ChildRemoved += SeriesItemsControl_ChildRemoved;
                this.PART_SeriesItemsControl.Reset += PART_SeriesItemsControl_Reset;

                OnSeriesTemplateChanged();
                OnSeriesTemplateSelectorChanged();

                this.PART_SeriesItemsControl.ItemsSource = this.SeriesItemsSource;
            }


        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (sizeInfo.NewSize.NearlyEqual(sizeInfo.PreviousSize))
            {
                return;
            }

            if (sizeInfo.WidthChanged)
            {
                UpdateXPixelPerUnit();
            }

            if (sizeInfo.HeightChanged)
            {
                UpdateYPixelPerUnit();
            }

            foreach (var sr in GetSeries())
            {
                sr.RecalculateCoordinate();
            }


        }


        private void PART_SeriesItemsControl_Reset(object obj)
        {
            UpdateGlobalValueRange();
        }

        private void SeriesItemsControl_ChildRemoved(object arg1, FrameworkElement arg2)
        {
            UpdateGlobalValueRange();
        }

        private void SeriesItemTemplateApplied(object sender, DependencyObject root, int index)
        {
            if (root == null)
            {
                return;
            }

            var sr = root as SeriesControl;
            if (sr == null)
            {
                MvvmChartException.ThrowDataTemplateRootElementException("SeriesTemplate", typeof(SeriesControl));

            }

            sr.Owner = this;

            sr.ValueRangeChanged += Sr_ValueRangeChanged;

            sr.RecalculateCoordinate();

            if (this.StackMode == StackMode.NotStacked)
            {
                sr.UpdateValueRange();
                sr.Refresh();

            }
        }

        private void ValidateData()
        {
            foreach (var seriesHost in GetSeries())
            {
                seriesHost.ValidateData();
            }


        }

        internal void UpdateValueRange()
        {

            if (GetSeries().Any(sr => sr.ItemsSource == null))
            {
                return;
            }

            foreach (var seriesHost in GetSeries())
            {

                seriesHost.UpdateValueRange();

            }
        }

        internal void Refresh()
        {
            if (GetSeries().Any(sr => sr.ItemsSource == null))
            {
                return;
            }

            foreach (var seriesHost in GetSeries())
            {
                seriesHost.Refresh();
            }
        }

        #region SeriesItemsSource

        /// <summary>
        /// Represents the data for a list of series(<see cref="SeriesControl"/>). 
        /// </summary>
        public IList SeriesItemsSource => this.Owner?.SeriesItemsSource;

        public void OnSeriesItemsSourceChanged(IList oldValue, IList newValue)
        {
            VerifyAccess();
            ValidateData();

            RefreshItemIndexMap();

            if (oldValue is INotifyCollectionChanged oldItemsSource)
            {
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .AddHandler(oldItemsSource, "CollectionChanged", SeriesItemsSource_CollectionChanged);
            }

            if (newValue is INotifyCollectionChanged newItemsSource)
            {
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .AddHandler(newItemsSource, "CollectionChanged", SeriesItemsSource_CollectionChanged);
            }
        }

        private void SeriesItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshItemIndexMap();
        }


        private readonly Dictionary<object, int> itemIndexMap = new Dictionary<object, int>();
        private void RefreshItemIndexMap()
        {
            this.itemIndexMap.Clear();
            for (int i = 0; i < this.SeriesItemsSource.Count; i++)
            {
                this.itemIndexMap.Add(this.SeriesItemsSource[i], i);
            }
        }

        #endregion

        #region StackMode
        public StackMode StackMode => this.Owner.SeriesStackMode;
        internal void OnStackModeChanged()
        {
            if (!this.IsSeriesCollectionChanging)
            {
                this.IsSeriesCollectionChanging = true;
                try
                {
                    Reset();
                    ValidateData();
                    UpdateValueRange();
                    Refresh();
                }
                finally
                {
                    this.IsSeriesCollectionChanging = false;
                }
            }
            else
            {
                Reset();
                ValidateData();
                UpdateValueRange();
                Refresh();
            }
        }
        #endregion

        #region Global Data Range
        private Range2D _globalValueRange = Range2D.Empty;
        /// <summary>
        /// The value Range of all series data
        /// </summary>
        public Range2D GlobalValueRange
        {
            get { return this._globalValueRange; }
            set
            {
                var old = this.GlobalValueRange;
                if (!this._globalValueRange.Equals(value))
                {
                    this._globalValueRange = value;

                    if (old.XRange != value.XRange)
                    {
                        UpdateActualPlottingXValueRange();
                    }

                    if (old.YRange != value.YRange)
                    {
                        UpdateActualPlottingYValueRange();
                    }
                }

            }
        }

        public double XStartValue => this.XPlottingRange.FullRange.Min;
        public double YStartValue => this.YPlottingRange.FullRange.Min;

        private double _yBaseValue;
        public double YBaseValue
        {
            get { return this._yBaseValue; }
            internal set
            {
                if (this._yBaseValue != value)
                {
                    this._yBaseValue = value;
                    UpdateYBaseCoordinate();
                }
            }
        }

        public double YBaseCoordinate { get; set; } = double.NaN;

        private void UpdateYBaseCoordinate()
        {
            this.YBaseCoordinate = (this.YBaseValue - this.YPlottingRange.FullRange.Min) * this.YPixelPerUnit;
        }

        internal void UpdateYBaseValue()
        {
            this.YBaseValue = this.Owner.YBaseValue;

            this.UpdateValueRange();
            this.Refresh();
        }


        private void Sr_ValueRangeChanged(ISeriesControl sr, Range2D obj)
        {
            UpdateGlobalValueRange();
        }



        private void UpdateGlobalValueRange()
        {
            if (this.SeriesCount == 0)
            {
                this.GlobalValueRange = Range2D.Empty;
                return;
            }

            double minX = double.MaxValue, minY = double.MaxValue,
                maxX = double.MinValue, maxY = double.MinValue;

            foreach (var sr in GetSeries())
            {
                if (sr.ValueRange.IsEmpty)
                {
                    this.GlobalValueRange = Range2D.Empty;

                    return;
                }

                minX = Math.Min(minX, sr.ValueRange.MinX);
                maxX = Math.Max(maxX, sr.ValueRange.MaxX);

                minY = Math.Min(minY, sr.ValueRange.MinY);
                maxY = Math.Max(maxY, sr.ValueRange.MaxY);
            }

            this.GlobalValueRange = new Range2D(minX, maxX, minY, maxY);

        }

        #endregion

        private double _xPixelPerUnit = double.NaN;
        public double XPixelPerUnit
        {
            get { return this._xPixelPerUnit; }
            set
            {
                if (this._xPixelPerUnit != value)
                {
                    this._xPixelPerUnit = value;
                    //todo: this.BarSeries?.UpdateBarWidth(this.MinXValueGap, this.XPixelPerUnit);

                }

            }
        }

        private double _yPixelPerUnit = double.NaN;
        public double YPixelPerUnit
        {
            get { return this._yPixelPerUnit; }
            set
            {
                if (this._yPixelPerUnit != value)
                {
                    this._yPixelPerUnit = value;
                    UpdateYBaseCoordinate();
                }
            }
        }

        private void UpdateXPixelPerUnit()
        {
            if (this.XPlottingRange.IsEmpty ||
                this.RenderSize.IsInvalid())
            {
                this.XPixelPerUnit = double.NaN;
                return;
            }

            this.XPixelPerUnit = this.ActualWidth / this.XPlottingRange.FullRange.Span;
        }

        private void UpdateYPixelPerUnit()
        {
            if (this.YPlottingRange.IsEmpty ||
               this.RenderSize.IsInvalid())
            {
                this.YPixelPerUnit = double.NaN;

                return;
            }


            this.YPixelPerUnit = this.ActualHeight / this.YPlottingRange.FullRange.Span;

        }

        #region Plotting Data value Range

        private PlottingRange _xPlottingRange = PlottingRange.Empty;
        public PlottingRange XPlottingRange
        {
            get { return this._xPlottingRange; }
            set
            {
                var oldValue = this.XPlottingRange.ActualRange;
                if (!this._xPlottingRange.Equals(value))
                {
                    this._xPlottingRange = value;

                    UpdateXPixelPerUnit();

                    foreach (var sr in GetSeries())
                    {
                        sr.RecalculateCoordinate();
                    }

                    if (oldValue != value.ActualRange)
                    {
                        this.ActualXPlottingRangeChanged?.Invoke(value.ActualRange);
                    }
                }
            }
        }

        private PlottingRange _yPlottingRange = PlottingRange.Empty;
        public PlottingRange YPlottingRange
        {
            get { return this._yPlottingRange; }
            set
            {
                var oldValue = this.YPlottingRange.ActualRange;
                if (!this._yPlottingRange.Equals(value))
                {
                    this._yPlottingRange = value;
                    UpdateYBaseCoordinate();
                    UpdateYPixelPerUnit();

                    foreach (var sr in GetSeries())
                    {
                        sr.RecalculateCoordinate();
                    }

                    if (oldValue != value.ActualRange)
                    {
                        this.ActualYPlottingRangeChanged?.Invoke(value.ActualRange);
                    }


                }
            }
        }


        internal void OnXValuePaddingChanged()
        {
            this.XPlottingRange = PlottingRangeHelper.PlottingRange(this.XPlottingRange.ActualRange, this.Owner.XValuePadding);
        }

        internal void OnYValuePaddingChanged()
        {
            this.YPlottingRange = PlottingRangeHelper.PlottingRange(this.YPlottingRange.ActualRange, this.Owner.YValuePadding);
        }

        internal void UpdateActualPlottingXValueRange()
        {
            double min = !this.Owner.XMinimum.IsNaN() ? this.Owner.XMinimum : this.GlobalValueRange.MinX;
            double max = !this.Owner.XMaximum.IsNaN() ? this.Owner.XMaximum : this.GlobalValueRange.MaxX;

            if (min.IsNaN() && max.IsNaN())
            {
                this.XPlottingRange = PlottingRange.Empty;
                return;
            }

            Range actualRange = new Range(min, max);
            this.XPlottingRange = PlottingRangeHelper.PlottingRange(actualRange, this.Owner.XValuePadding);


        }

        internal void UpdateActualPlottingYValueRange()
        {

            double min = !this.Owner.YMinimum.IsNaN() ? this.Owner.YMinimum : this.GlobalValueRange.MinY;
            double max = !this.Owner.YMaximum.IsNaN() ? this.Owner.YMaximum : this.GlobalValueRange.MaxY;

            if (min.IsNaN() && max.IsNaN())
            {
                this.YPlottingRange = PlottingRange.Empty;
                return;
            }
            Range actualRange = GetProperPlottingYRange(new Range(min, max));
            this.YPlottingRange = PlottingRangeHelper.PlottingRange(actualRange, this.Owner.YValuePadding);
        }

        private Range GetProperPlottingYRange(Range newRange)
        {

            switch (this.StackMode)
            {
                case StackMode.NotStacked:
                    return newRange;

                case StackMode.Stacked:
                    return new Range(0.00, newRange.Max);

                case StackMode.Stacked100:
                    return new Range(0.00, 1.00);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion


        internal void OnSeriesTemplateSelectorChanged()
        {
            if (this.PART_SeriesItemsControl != null)
            {
                this.PART_SeriesItemsControl.ItemTemplateSelector = this.Owner.SeriesTemplateSelector;
            }
        }

        internal void OnSeriesTemplateChanged()
        {
            if (this.PART_SeriesItemsControl != null)
            {
                this.PART_SeriesItemsControl.ItemTemplate = this.Owner.SeriesTemplate;
            }
        }

        internal void Reset()
        {
            this.XPixelPerUnit = double.NaN;
            this.YPixelPerUnit = double.NaN;
            foreach (var sr in GetSeries())
            {
                sr.Reset();
            }
            switch (this.StackMode)
            {
                case StackMode.NotStacked:
                    this.YPlottingRange = PlottingRange.Empty;
                    break;
                case StackMode.Stacked:
                    this.YPlottingRange = PlottingRangeHelper.PlottingRange(new Range(0, double.NaN), this.Owner.YValuePadding);
                    break;
                case StackMode.Stacked100:
                    this.YPlottingRange = PlottingRangeHelper.PlottingRange(new Range(0, 1), this.Owner.YValuePadding);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Debug.Assert(this.GlobalValueRange.IsEmpty);
        }
    }
}
