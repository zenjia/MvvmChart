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

        public bool IsSeriesCollectionChanging { get; internal set; }

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
                //this.PART_SeriesItemsControl.ItemAdded += PART_SeriesItemsControl_ItemAdded;
                this.PART_SeriesItemsControl.ChildRemoved += PartSeriesChildrenControlChildRemoved;
                //this.PART_SeriesItemsControl.ItemReplaced += PART_SeriesItemsControl_ItemReplaced;
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
        }

        private void PartSeriesChildrenControlChildRemoved(object arg1, FrameworkElement arg2)
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
                MvvmChartException.ThrowDataTemplateRootElementException(nameof(this.SeriesTemplate), typeof(SeriesControl));
 
            }

            sr.Owner = this;

            sr.XRangeChanged += Sr_XValueRangeChanged;
            sr.YRangeChanged += Sr_YValueRangeChanged;

            sr.OnPlottingXValueRangeChanged(this.XPlottingRange);
            sr.OnPlottingYValueRangeChanged(this.YPlottingRange);

            //sr.UpdateValueRange();
            ((SeriesControl)sr).Refresh();

        }

        private void ValidateData()
        {
            foreach (var seriesHost in GetSeries())
            {
                seriesHost.ValidateData();
            }

            //if (!this.IsXAxisCategory && (this.StackMode == StackMode.NotStacked))
            //{
            //    return;
            //}

            //string strReason = this.StackMode != StackMode.NotStacked ? $"In {this.StackMode} mode" : "If the XAxis of a Chart is CategoryAxis";
            //SeriesHost prev = null;
            //foreach (var seriesHost in this.GetSeries())
            //{
            //    if (seriesHost.ItemsSource == null)
            //    {
            //        continue;
            //    }

            //    if (prev == null)
            //    {
            //        prev = seriesHost;
            //        continue;
            //    }

            //    if (seriesHost.ItemsSource.Count != prev.ItemsSource.Count)
            //    {
            //        throw new MvvmChartModelDataException($"{strReason}, the item count of all series in a Chart must be same!");
            //    }

            //    for (int i = 0; i < seriesHost.ItemsSource.Count; i++)
            //    {
            //        if (this.IsXAxisCategory)
            //        {
            //            var x1 = seriesHost.GetXRawValueForItem(seriesHost.ItemsSource[i]);
            //            var x2 = seriesHost.GetXRawValueForItem(prev.ItemsSource[i]);
            //            if (x1 == null || x2 == null)
            //            {
            //                throw new MvvmChartModelDataException("An Item's X value of the series ItemsSource cannot be null!");
            //            }

            //            if (!x1.Equals(x2))
            //            {
            //                throw new MvvmChartModelDataException($"{strReason}, the item's x value in the same index of all series in a Chart must be same!");
            //            }
            //        }
            //        else
            //        {
            //            var x1 = seriesHost.GetXValueForItem(seriesHost.ItemsSource[i]);
            //            var x2 = seriesHost.GetXValueForItem(prev.ItemsSource[i]);

            //            if (!x1.NearlyEqual(x2))
            //            {
            //                throw new MvvmChartModelDataException($"{strReason}, the item's x value in the same index of all series in a Chart must be same!");
            //            }
            //        }

            //    }


            //}
        }

        internal void Refresh()
        {
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
                    Reset();
                    ValidateData();
                    Refresh();
                }

            }
        }
        #endregion

        #region Global Data Range
        private Range _globalYValueRange = Range.Empty;
        /// <summary>
        /// The dependent value Range(min & max) of all series data
        /// </summary>
        public Range GlobalYValueRange
        {
            get { return this._globalYValueRange; }
            set
            {
                if (this._globalYValueRange != value)
                {
                    this._globalYValueRange = value;


                    this.GlobalYValueRangeChanged?.Invoke(value);
                }
            }
        }

        private Range _globalXValueRange = Range.Empty;
        /// <summary>
        /// The independent value Range(min & max) of all series data
        /// </summary>
        public Range GlobalXValueRange
        {
            get { return this._globalXValueRange; }
            set
            {
                if (this._globalXValueRange != value)
                {
                    this._globalXValueRange = value;


                    this.GlobalXValueRangeChanged?.Invoke(value);
                }
            }
        }

        public event Action<Range> GlobalXValueRangeChanged;
        public event Action<Range> GlobalYValueRangeChanged;

        private void Sr_XValueRangeChanged(ISeriesControl sr, Range obj)
        {
            UpdateGlobalValueRange();
        }

        private void Sr_YValueRangeChanged(ISeriesControl sr, Range obj)
        {
            UpdateGlobalValueRange();
        }

        private void UpdateGlobalValueRange()
        {
            if (this.SeriesCount == 0)
            {
                this.GlobalXValueRange = Range.Empty;
                this.GlobalYValueRange = Range.Empty;
                return;
            }

            double minX = double.MaxValue, minY = double.MaxValue,
                maxX = double.MinValue, maxY = double.MinValue;

            foreach (var sr in GetSeries())
            {
                if (sr.XValueRange.IsEmpty || sr.YValueRange.IsEmpty)
                {
                    this.GlobalXValueRange = Range.Empty;
                    this.GlobalYValueRange = Range.Empty;

                    return;
                }

                minX = Math.Min(minX, sr.XValueRange.Min);
                maxX = Math.Max(maxX, sr.XValueRange.Max);

                minY = Math.Min(minY, sr.YValueRange.Min);
                maxY = Math.Max(maxY, sr.YValueRange.Max);

            }

            this.GlobalXValueRange = new Range(minX, maxX);
            this.GlobalYValueRange = new Range(minY, maxY);



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

            Debug.Assert(this.GlobalXValueRange == Range.Empty);
            Debug.Assert(this.GlobalYValueRange == Range.Empty);
        }
    }
}
