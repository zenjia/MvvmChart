using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace MvvmCharting.Axis
{
    public enum XAxisPlacement
    {
        Bottom,
        Top
    }

    public class XAxis : AxisBase
    {
        static XAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XAxis), new FrameworkPropertyMetadata(typeof(XAxis)));
        }



        public XAxisPlacement XAxisPlacement
        {
            get { return (XAxisPlacement)GetValue(XAxisPlacementProperty); }
            set { SetValue(XAxisPlacementProperty, value); }
        }
        public static readonly DependencyProperty XAxisPlacementProperty =
            DependencyProperty.Register("XAxisPlacement", typeof(XAxisPlacement), typeof(XAxis), new PropertyMetadata(XAxisPlacement.Bottom, OnXAxisPlacementPropertyChanged));

        private static void OnXAxisPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((XAxis)d).OnAxisPlacementChanged(); 
        }

 


        protected override double GetActualLength()
        {
            if (this.Owner==null)
            {
                return Double.NaN;
            }
 
            return this.ActualWidth - (this.Padding.Left + this.Padding.Right);
        }

        protected override Range GetChartRange()
        {
            return ((IXAxisOwner) this.Owner)?.PlotAreaXDataRange??Range.Empty;
        }


        protected override void OnOwnerChanged(IAxisOwner oldValue, IAxisOwner newValue)
        {
            base.OnOwnerChanged(oldValue, newValue);

            if (oldValue !=null)
            {
                ((IXAxisOwner)oldValue).PlotAreaXRangeChanged -= OnChartPlotAreaRangeChanged;
            }

            if (newValue != null)
            {
                ((IXAxisOwner)newValue).PlotAreaXRangeChanged += OnChartPlotAreaRangeChanged;
            }

        }

        protected override void SynchronizePadding()
        {
            Thickness chartPadding = this.Owner.Padding;
            if (!chartPadding.Left.NearlyEqual(this.Padding.Left) ||
                !chartPadding.Right.NearlyEqual(this.Padding.Right))
            {
                this.Padding = new Thickness(chartPadding.Left, this.Padding.Top, chartPadding.Right, this.Padding.Bottom);
            }
           
        }

        protected override void SynchronizeBorderThickness()
        {
            Thickness chartBorderThickness = this.Owner.BorderThickness;
            if (!chartBorderThickness.Left.NearlyEqual(this.Padding.Left) || 
                !chartBorderThickness.Right.NearlyEqual(this.Padding.Right))
            {
                this.BorderThickness = new Thickness(chartBorderThickness.Left, this.Padding.Top, chartBorderThickness.Right, this.Padding.Bottom);
            }
        }

        protected override void SynchronizeMargin()
        {
            Thickness chartMargin = this.Owner.Margin;
            if (!chartMargin.Left.NearlyEqual(this.Padding.Left) || 
                !chartMargin.Right.NearlyEqual(this.Padding.Right))
            {
                this.Margin = new Thickness(chartMargin.Left, this.Padding.Top, chartMargin.Right, this.Padding.Bottom);
            }
        }


        protected override void DoUpdateAxisItemOffset()
        {
            var span = this.ActualValues.ActualRangeSpan;
            var uLen = this.ActualValues.ActualLength / span;
            foreach (var item in this.AxisDataOffsets)
            {
                item.Offset = (item.Data - this.ActualValues.CurrentRange.Min) * uLen;
            }
        }

        protected override void OnAxisTickChanged()
        {
            var range = GetChartRange();
            ((IXAxisOwner)this.Owner).OnXAxisTickChanged(this.AxisDataOffsets
                .Where(x => range.IsInRange(x.Data))
                .Select(x => x.Offset));  
        }
    }
}