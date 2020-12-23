using System;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MvvmChart.Common;

namespace MvvmCharting.Axis
{
    public enum YAxisPlacement
    {
        Left,
        Right
    }


    public class YAxis : AxisBase
    {
        static YAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(YAxis), new FrameworkPropertyMetadata(typeof(YAxis)));
        }

        public YAxisPlacement YAxisPlacement
        {
            get { return (YAxisPlacement)GetValue(YAxisPlacementProperty); }
            set { SetValue(YAxisPlacementProperty, value); }
        }
        public static readonly DependencyProperty YAxisPlacementProperty =
            DependencyProperty.Register("YAxisPlacement", typeof(YAxisPlacement), typeof(YAxis), new PropertyMetadata(YAxisPlacement.Left, OnYAxisPlacementPropertyChanged));

        private static void OnYAxisPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
             ((YAxis)d).OnAxisPlacementChanged();
        }


        protected override double GetActualLength()
        {
            if (this.Owner == null)
            {
                return Double.NaN;
            }
            return this.ActualHeight - this.Padding.Top - this.Padding.Bottom;
        }

        protected override Range GetChartRange()
        {
            return ((IYAxisOwner)this.Owner)?.PlotAreaYDataRange ?? Range.Empty;
        }

        protected override void OnOwnerChanged(IAxisOwner oldValue, IAxisOwner newValue)
        {
            base.OnOwnerChanged(oldValue, newValue);

            if (oldValue != null)
            {
                ((IYAxisOwner)oldValue).PlotAreaYRangeChanged -= OnChartPlotAreaRangeChanged;
            }

            if (newValue != null)
            {
                ((IYAxisOwner)newValue).PlotAreaYRangeChanged += OnChartPlotAreaRangeChanged;
            }

        }

        protected override void SynchronizePadding()
        {
            Thickness chartPadding = this.Owner.Padding;
            if (!chartPadding.Top.NearlyEqual(this.Padding.Top) || !chartPadding.Bottom.NearlyEqual(this.Padding.Bottom))
            {
                this.Padding = new Thickness(this.Padding.Left, chartPadding.Top, this.Padding.Right,  chartPadding.Bottom);
            }

        }

        protected override void SynchronizeBorderThickness()
        {
            Thickness chartBorderThickness = this.Owner.BorderThickness;
            if (!chartBorderThickness.Top.NearlyEqual(this.Padding.Top) || !chartBorderThickness.Bottom.NearlyEqual(this.Padding.Bottom))
            {
                this.BorderThickness = new Thickness(this.Padding.Left, chartBorderThickness.Top, this.Padding.Right, chartBorderThickness.Bottom);
            }
        }

        protected override void SynchronizeMargin()
        {
            Thickness chartMargin = this.Owner.Margin;
            if (!chartMargin.Top.NearlyEqual(this.Padding.Top) || !chartMargin.Bottom.NearlyEqual(this.Padding.Bottom))
            {
                this.Margin = new Thickness(this.Padding.Left, chartMargin.Top, this.Padding.Right, chartMargin.Bottom);
            }
        }


        /// <summary>
        /// 1.ActualRange
        /// 2.ActualLength: a.ActualWidth or ActualHeight; b.Padding
        /// 3.item.DataValue
        /// </summary>
        protected override void DoUpdateAxisItemOffset()
        {
            var span = this.ActualValues.ActualRangeSpan;
            var uLen = this.ActualValues.ActualLength / span;
            foreach (var item in this.AxisDataOffsets)
            {
                item.Offset = this.ActualValues.ActualLength - (item.Data - this.ActualValues.CurrentRange.Min) * uLen;
            }
        }

        protected override void OnAxisTickChanged()
        {
            var range = GetChartRange();

            ((IYAxisOwner)this.Owner).OnYAxisTickChanged(this.AxisDataOffsets
                .Where(x=>range.IsInRange(x.Data))
                .Select(x => x.Offset));
        }
    }
}