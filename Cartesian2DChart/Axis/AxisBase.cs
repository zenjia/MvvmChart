using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MvvmCharting.Axis
{

    /// <summary>
    /// numeric(DateTime), linear axis
    /// </summary>
    [TemplatePart(Name = "PART_AxisItemsControl", Type = typeof(ItemsControlEx))]
    public abstract class AxisBase : Control
    {
        private static readonly string sPART_AxisItemsControl = "PART_AxisItemsControl";

        private ItemsControlEx PART_AxisItemsControl;
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.PART_AxisItemsControl != null)
            {
                this.PART_AxisItemsControl.ItemTemplateApplied -= PartAxisItemsControlItemTemplateApplied;
            }

            this.PART_AxisItemsControl = (ItemsControlEx)GetTemplateChild(sPART_AxisItemsControl);
            this.PART_AxisItemsControl.ItemsSource = this.AxisDataOffsets;
            this.PART_AxisItemsControl.ItemTemplateApplied += PartAxisItemsControlItemTemplateApplied; ;
            TryLoadAxisItemDataOffsets();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
 
            if (e.Property == BorderThicknessProperty)
            {
                SynchronizeBorderThickness();
            }
            else if (e.Property == PaddingProperty)
            {
                SynchronizePadding();
            }
            else if (e.Property == MarginProperty)
            {
                SynchronizeMargin();
            }
        }

        protected AxisBase()
        {
            this.AxisDataOffsets = new ObservableCollection<DataOffset>();
            this.SizeChanged += AxisBase_SizeChanged;


        }

        private void AxisBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            UpdateActualValues();

        }

        public double TickInterval
        {
            get { return (double)GetValue(TickIntervalProperty); }
            set { SetValue(TickIntervalProperty, value); }
        }
        public static readonly DependencyProperty TickIntervalProperty =
            DependencyProperty.Register("TickInterval", typeof(double), typeof(AxisBase), new PropertyMetadata(double.NaN, OnTickIntervalPropertyChanged));

        private static void OnTickIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ((AxisBase)d).UpdateActualValues(); ;
        }


        public int TickCount
        {
            get { return (int)GetValue(TickCountProperty); }
            set { SetValue(TickCountProperty, value); }
        }
        public static readonly DependencyProperty TickCountProperty =
            DependencyProperty.Register("TickCount", typeof(int), typeof(AxisBase), new PropertyMetadata(5, OnTickCountPropertyChanged));

        private static void OnTickCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ((AxisBase)d).UpdateActualValues();
        }




        public IList<double> ExplicitTicks
        {
            get { return (IList<double>)GetValue(ExplicitTicksProperty); }
            set { SetValue(ExplicitTicksProperty, value); }
        }
        public static readonly DependencyProperty ExplicitTicksProperty =
            DependencyProperty.Register("ExplicitTicks", typeof(IList<double>), typeof(AxisBase), new PropertyMetadata(null, OnExplicitTicksPropertyChanged));

        private static void OnExplicitTicksPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisBase)d).OnExplicitTicksChanged((IList<double>)e.OldValue, (IList<double>)e.NewValue);
        }

        private void OnExplicitTicksChanged(IList<double> oldValue, IList<double> newValue)
        {
            TryLoadAxisItemDataOffsets();
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner(typeof(AxisBase));



        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            ItemsControl.ItemTemplateSelectorProperty.AddOwner(typeof(AxisBase));




        public IAxisOwner Owner
        {
            get { return (IAxisOwner)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(IAxisOwner), typeof(AxisBase), new PropertyMetadata(null, OnAxisOwnerPropertyChanged));

        private static void OnAxisOwnerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisBase)d).OnOwnerChanged();
        }



        protected virtual void OnOwnerChanged()
        {

            if (this.Owner == null)
            {
                return;
            }


            SynchronizePadding();
            SynchronizeBorderThickness();
            SynchronizeMargin();

            UpdateActualValues();

            this.Owner.PaddingChanged += Owner_PaddingChanged; //should be weak??
            this.Owner.BorderThicknessChanged += Owner_BorderThicknessChanged;
            this.Owner.MarginChanged += Owner_MarginChanged;
        }

        private void Owner_PaddingChanged(Thickness newPadding)
        {
            SynchronizePadding();
            UpdateActualValues();
        }

        private void Owner_MarginChanged(Thickness obj)
        {
            SynchronizeMargin();
            UpdateActualValues();
        }

        private void Owner_BorderThicknessChanged(Thickness obj)
        {
            SynchronizeBorderThickness();
            UpdateActualValues();
        }

        private AxisActualValues _actualValues;
        internal AxisActualValues ActualValues
        {
            get { return this._actualValues; }
            set
            {

                var old = this.ActualValues;
                if (old == null || !old.Equals(value))
                {
                    this._actualValues = value;

                    TryLoadAxisItemDataOffsets();
                }


            }
        }


        public ObservableCollection<DataOffset> AxisDataOffsets { get; }



        private void UpdateActualValues()
        {
            Range range = GetChartRange();
            var length = GetActualLength();
            if (range.IsEmpty || length.IsNaNOrZero())
            {


                return;
            }

            this.ActualValues = new AxisActualValues(this.TickCount, this.TickInterval, range, length);
        }


        protected void OnChartPlotAreaRangeChanged(Range newRange)
        {
           
            UpdateActualValues();
        }


        protected abstract double GetActualLength();
        protected abstract Range GetChartRange();
        protected abstract void SynchronizePadding();
        protected abstract void SynchronizeBorderThickness();
        protected abstract void SynchronizeMargin();
        protected abstract void DoUpdateAxisItemOffset();


        private void LoadAxisDataOffsets(double startValue, double tickInterval, int tickCount)
        {
            var chartRange = GetChartRange();

            var arr = Enumerable.Range(0, tickCount)
                .Select(i => startValue + i * tickInterval)
                .Where(x => chartRange.IsInRange(x))
                .ToArray();

            DoLoadAxisDataOffsets(arr);
        }

        private void DoLoadAxisDataOffsets(IList<double> source)
        {

            var oldCt = this.AxisDataOffsets.Count;
            var newCt = source.Count;
            if (oldCt > newCt)
            {
                this.AxisDataOffsets.RemoveRange(newCt, oldCt - newCt);
            }

            for (int i = 0; i < source.Count; i++)
            {
                var newValue = source[i];
                if (i < oldCt)
                {
                    DataOffset item = this.AxisDataOffsets[i];
                    item.Data = newValue;
                }
                else
                {
                    DataOffset item = new DataOffset();
                    item.Data = newValue;
                    this.AxisDataOffsets.Add(item);
                }
            }


   

        }

        protected abstract void OnAxisTickChanged();


        /// <summary>
        /// 1.Range是否null，是否不同
        /// 2.ActualLength: a.ActualWidth or ActualHeight. b. Padding
        /// 3.this.PART_root是否为null，OnApplyTemplate
        /// 4.Tick的设置：TickCount，TickInterval
        /// </summary>
        /// <param name="newRange"></param>
        public bool TryLoadAxisItemDataOffsets()
        {
            AxisActualValues values = this.ActualValues;
            if (values == null || !values.CanUpdateAxisItems())
            {
                return false;
            }

            if (values.Equals(this._currentActualValues))
            {
                return false;
            }

            if (this.ExplicitTicks != null)
            {
                DoLoadAxisDataOffsets(this.ExplicitTicks);
            }
            else
            {
                LoadAxisDataOffsets(values.CurrentRange.Min, values.ActualTickInterval, values.ActualTickCount);

            }

            UpdateAxisItemOffset();

            if (this.AxisDataOffsets.Any())
            {
                Debug.WriteLine(GetType() + $"  ActualLength = {GetActualLength()}");
                Debug.WriteLine(GetType() + $"  first = {this.AxisDataOffsets.First().Offset.ToString("F2")}, last = {this.AxisDataOffsets.Last().Offset.ToString("F2")},");
            }

            this._currentActualValues = values;
            return true;
        }


        private void UpdateAxisItemOffset()
        {

            //if the length of Axis is NaN or zero, we can't calculate its item's offset
            if (!this.ActualValues.CanUpdateAxisItemsOffset())
            {
                return;
            }

            DoUpdateAxisItemOffset();
            OnAxisTickChanged();
        }


        private AxisActualValues _currentActualValues;


        private void PartAxisItemsControlItemTemplateApplied(object sender, DependencyObject root)
        {
            if (!(root is AxisItem))
            {
                throw new MvvmChartUnexpectedTypeException(this.PART_AxisItemsControl.Name + $"  The root item of ItemTemplate of an axis must be based on '{typeof(AxisItem)}'!");
            }

        }

 



        public event Action<AxisBase> AxisPlacementChanged;

        protected void OnAxisPlacementChanged()
        {
            this.AxisPlacementChanged?.Invoke(this);

        }
    }

}
