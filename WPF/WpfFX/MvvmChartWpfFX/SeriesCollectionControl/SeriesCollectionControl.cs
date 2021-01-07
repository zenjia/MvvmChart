using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MvvmCharting.Common;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX.Series
{

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
                if (IsSeriesCollectionChanging && !value)
                {
                    UpdateValueRange();
                }

                if (this._isSeriesCollectionChanging != value)
                {
                    this._isSeriesCollectionChanging = value;
                    if (!value)
                    {
                        this.Refresh();
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

                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemTemplateProperty,
                    new Binding(nameof(this.SeriesTemplate)) { Source = this });
                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemTemplateSelectorProperty,
                    new Binding(nameof(this.SeriesTemplateSelector)) { Source = this });
                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemsSourceProperty,
                    new Binding(nameof(this.SeriesItemsSource)) { Source = this });
            }


        }

        private void PART_SeriesItemsControl_Reset(object obj)
        {
            UpdateGlobalValueRange();
            UpdateGlobalRawValueRange();
        }

        private void SeriesItemsControl_ChildRemoved(object arg1, FrameworkElement arg2)
        {
            UpdateGlobalValueRange();
            UpdateGlobalRawValueRange();
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
                MvvmChartException.ThrowDataTemplateRootElementException(nameof(this.SeriesTemplate), typeof(SeriesControl));

            }

            sr.Owner = this;

            sr.ValueRangeChanged += Sr_ValueRangeChanged;
            sr.RawValueRangeChanged += Sr_RawValueRangeChanged;

            sr.OnPlottingXValueRangeChanged(this.XPlottingRange);
            sr.OnPlottingYValueRangeChanged(this.YPlottingRange);

           
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

            if (GetSeries().Any(sr=>sr.ItemsSource==null))
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
        public IList SeriesItemsSource
        {
            get { return (IList)GetValue(SeriesItemsSourceProperty); }
            set { SetValue(SeriesItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty SeriesItemsSourceProperty =
            DependencyProperty.Register("SeriesItemsSource", typeof(IList), typeof(SeriesCollectionControl), new PropertyMetadata(null, OnSeriesItemsSourcePropertyChanged));

        private static void OnSeriesItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesCollectionControl)d).OnSeriesItemsSourceChanged((IList)e.OldValue, (IList)e.NewValue);
        }



        private void OnSeriesItemsSourceChanged(IList oldValue, IList newValue)
        {
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
        private StackMode _stackMode = StackMode.NotStacked;
        public StackMode StackMode
        {
            get { return this._stackMode; }
            internal set
            {
                if (this._stackMode != value)
                {
                    this._stackMode = value;

                    if (!this.IsSeriesCollectionChanging)
                    {
                        this.IsSeriesCollectionChanging = true;
                        try
                        {
                            Reset();
                            UpdateGlobalRawValueRange();
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
                        UpdateGlobalRawValueRange();
                        ValidateData();
                        UpdateValueRange();
                        Refresh();
                    }

                }

            }
        }
        #endregion

        #region Global Data Range
        private Range2D _globalValueRange;
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
                    this.GlobalValueRangeChanged?.Invoke(old, value);
                }

            }
        }

        private Range2D _globalRawValueRange;
        /// <summary>
        /// The value Range of all series data
        /// </summary>
        public Range2D GlobalRawValueRange
        {
            get { return this._globalRawValueRange; }
            set
            {

                if (!this._globalRawValueRange.Equals(value))
                {
                    this._globalRawValueRange = value;
                }

            }
        }


        public event Action<Range2D, Range2D> GlobalValueRangeChanged;

        private void Sr_ValueRangeChanged(ISeriesControl sr, Range2D obj)
        {
            UpdateGlobalValueRange();
        }

        private void Sr_RawValueRangeChanged(ISeriesControl sr, Range2D obj)
        {
            UpdateGlobalRawValueRange();
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

        private void UpdateGlobalRawValueRange()
        {
            if (this.StackMode == StackMode.NotStacked || this.SeriesCount == 0)
            {
                this.GlobalRawValueRange = Range2D.Empty;

                return;
            }

            double minX = double.MaxValue, minY = double.MaxValue,
                maxX = double.MinValue, maxY = double.MinValue;

            foreach (var sr in GetSeries())
            {
                if (sr.RawValueRange.IsEmpty)
                {
                    sr.UpdateRawValueRange(sr.ItemsSource);
                }

                if (sr.RawValueRange.IsEmpty)
                {
                    this.GlobalRawValueRange = Range2D.Empty;
                    return;
                }

                minX = Math.Min(minX, sr.RawValueRange.MinX);
                maxX = Math.Max(maxX, sr.RawValueRange.MaxX);

                minY = Math.Min(minY, sr.RawValueRange.MinY);
                maxY = Math.Max(maxY, sr.RawValueRange.MaxY);
            }

            this.GlobalRawValueRange = new Range2D(minX, maxX, minY, maxY);

        }
        #endregion

        #region Plotting Data value Range

        private PlottingRange _xPlottingRange;
        public PlottingRange XPlottingRange
        {
            get { return this._xPlottingRange; }
            set
            {
                if (!this._xPlottingRange.Equals(value))
                {
                    this._xPlottingRange = value;
                    foreach (var sr in GetSeries())
                    {
                        sr.OnPlottingXValueRangeChanged(value);
                    }
                }
            }
        }

        private PlottingRange _yPlottingRange;
        public PlottingRange YPlottingRange
        {
            get { return this._yPlottingRange; }
            set
            {
                if (!this._yPlottingRange.Equals(value))
                {
                    this._yPlottingRange = value;

                    foreach (var sr in GetSeries())
                    {
                        sr.OnPlottingYValueRangeChanged(value);
                    }
                }
            }
        }

        internal virtual void SetPlottingValueRange(Orientation orientation, PlottingRange newValue)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    this.XPlottingRange = newValue;
                    break;
                case Orientation.Vertical:
                    this.YPlottingRange = newValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }
        #endregion

        #region SeriesDataTemplate & SeriesTemplateSelector
        public DataTemplate SeriesTemplate
        {
            get { return (DataTemplate)GetValue(SeriesTemplateProperty); }
            set { SetValue(SeriesTemplateProperty, value); }
        }
        public static readonly DependencyProperty SeriesTemplateProperty =
            DependencyProperty.Register("SeriesTemplate", typeof(DataTemplate), typeof(SeriesCollectionControl), new PropertyMetadata(null));

        public DataTemplateSelector SeriesTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SeriesTemplateSelectorProperty); }
            set { SetValue(SeriesTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty SeriesTemplateSelectorProperty =
            DependencyProperty.Register("SeriesTemplateSelector", typeof(DataTemplateSelector), typeof(SeriesCollectionControl), new PropertyMetadata(null));
        #endregion

        internal void Reset()
        {
            foreach (var sr in GetSeries())
            {
                sr.Reset();
            }

            Debug.Assert(this.GlobalValueRange.IsEmpty);
        }
    }
}
