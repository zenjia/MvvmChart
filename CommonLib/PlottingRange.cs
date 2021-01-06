using System.Windows;
using MvvmCharting.Common;

namespace MvvmCharting
{
    public struct PlottingRange
    {
        public static PlottingRange Empty = new PlottingRange(Range.Empty, 0, 0);

        public bool IsEmpty => this.ActualRange.IsEmpty;

        private Range _actualRange;
        public Range ActualRange
        {
            get { return this._actualRange; }
            private set
            {
                if (this._actualRange != value)
                {
                    this._actualRange = value;
                    //UpdatePlottingXValueRange();
                }

            }
        }

        private Range _fullRange;
        public Range FullRange
        {
            get { return this._fullRange; }
            private set
            {
                if (this._fullRange != value)
                {
                    this._fullRange = value;
                    // UpdatePlottingYValueRange();
                }
            }
        }

        public PlottingRange(Range actualRange, double minPadding, double maxPadding)
        {
            this._actualRange = actualRange;
            this._fullRange = new Range(actualRange.Min - minPadding, actualRange.Max + maxPadding);

            if (this.FullRange.Span <= 0)
            {
                throw new MvvmChartException($"Invalid value range: {this.FullRange}");
            }
        }

        public PlottingRange(PlottingRange oldValue, double minPadding, double maxPadding)
        {
            this._actualRange = oldValue.ActualRange;
            this._fullRange = new Range(this._actualRange.Min - minPadding, this._actualRange.Max + maxPadding);

            if (this.FullRange.Span <= 0)
            {
                throw new MvvmChartException($"Invalid value range: {this.FullRange}");
            }
        }

    }
}