using System.Windows;
using MvvmCharting.Common;

namespace MvvmCharting
{
    public struct PlottingRange
    {
        public static PlottingRange Empty = new PlottingRange(Range.Empty, 0, 0);

        public override string ToString()
        {
            return $"({FullRange}),({ActualRange})";
        }

        public bool IsEmpty => this.ActualRange.IsEmpty;

        public Range ActualRange { get;  }

        public Range FullRange { get;  }

        public PlottingRange(Range actualRange, double minPadding, double maxPadding)
        {
            this.ActualRange = actualRange;
            this.FullRange = new Range(actualRange.Min - minPadding, actualRange.Max + maxPadding);

            if (this.FullRange.Span <= 0)
            {
                throw new MvvmChartException($"Invalid value range: {this.FullRange}");
            }
        }

    }
}