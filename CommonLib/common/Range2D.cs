using System;

namespace MvvmCharting.Common
{
    public struct Range2D
    {
        public override string ToString()
        {
            return $"({this.XRange}, {this.YRange})";
        }

        public static Range2D ExpandRange(Range2D oldValue, double x, double y)
        {
            return new Range2D(Math.Min(oldValue.MinX, x), Math.Max(oldValue.MaxX, x),
                Math.Min(oldValue.MinY, y), Math.Max(oldValue.MaxY, y));
        }

        public Range2D(double xMin, double xMax, double yMin, double yMax)
        {
            this.XRange = new Range(xMin, xMax);
            this.YRange = new Range(yMin, yMax);
        }

        public Range2D(Range xRange, Range yRange)
        {
            this.XRange = xRange;
            this.YRange = yRange;
        }

        public Range XRange { get; }
        public Range YRange { get; }
        public double MinX => this.XRange.Min;
        public double MaxX => this.XRange.Max;
        public double MinY => this.YRange.Min;
        public double MaxY => this.YRange.Max;
        public bool IsEmpty => this.XRange.IsEmpty || this.YRange.IsEmpty;

        public static readonly Range2D Empty = new Range2D(Range.Empty, Range.Empty);

        public override bool Equals(object obj)
        {

            if (!(obj is Range2D))
            {
                return false;
            }

            return Equals((Range2D)obj);
        }

        public bool Equals(Range2D obj)
        {
            return this.XRange == obj.XRange && this.YRange == obj.YRange;
        }


        public bool IsOutOfRange(double minX, double maxX, double minY, double maxY)
        {
            return this.XRange.IsOutOfRange(minX, maxX) || this.YRange.IsOutOfRange(minY, maxY);
        }

        public bool IsInRange(double x, double y)
        {
            return this.XRange.IsInRange(x) && this.YRange.IsInRange(y);
        }

  
        public bool CanPointRemovingAffectRange(double x, double y)
        {
            return this.XRange.Min + double.Epsilon < x ||
                   x + double.Epsilon < this.XRange.Max ||
                   this.YRange.Min + double.Epsilon < y ||
                   y + double.Epsilon < this.YRange.Max;
        }
 
    }
}