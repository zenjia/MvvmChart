namespace MvvmCharting.Common
{
    public struct Range
    {
        public static readonly Range Empty = new Range(double.NaN, double.NaN);

        public double Min { get; }
        public double Max { get; }


        public Range(double min, double max)
        {
            this.Max = max;
            this.Min = min;
        }

        public bool IsEmpty
        {
            get { return this.Min.IsNaN() || this.Max.IsNaN(); }
        }

        public bool IsInvalid
        {
            get { return this.IsEmpty ||
                         double.IsInfinity(this.Min) ||
                         double.IsInfinity(this.Max) ||
                         this.Span.IsZero(); }
        }

        public override string ToString()
        {
            return $"({this.Min:F2}, {this.Max:F2})";
        }

        public double Span
        {
            get { return this.Max - this.Min; }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range))
            {
                return false;
            }

            return Equals((Range)obj);
        }
        public static bool Equals(Range range1, Range range2)
        {
            if (range1.IsEmpty)
                return range2.IsEmpty;
            return range1.Min.Equals(range2.Min) && range1.Max.Equals(range2.Max);
        }

        public bool Equals(Range other)
        {
            return Equals(this, other);
        }



        public override int GetHashCode()
        {
            return this.IsEmpty ? 0 : this.Min.GetHashCode() ^ this.Max.GetHashCode();
        }

        public static bool operator ==(Range point1, Range point2)
        {

            return point1.Equals(point2);
        }

        public static bool operator !=(Range point1, Range point2)
        {
            return !point1.Equals(point2);
        }

        //public static Range operator +(Range point1, Range point2)
        //{
        //    return new Range(point1.Min + point2.Min, point1.Max + point2.Max);
        //}

        //public static Range operator -(Range point1, Range point2)
        //{
        //    return new Range(point1.Min - point2.Min, point1.Max - point2.Max);
        //}

        public bool IsInRange(double d)
        {

            return d <= this.Max && d >= this.Min;
        }

   


        public bool IsOutOfRange(double min, double max)
        {
            if (this.IsEmpty)
            {
                return true;
            }
            return min < this.Min || max > this.Max;
        }
        
        public bool IsOutOfRange(Range newRange)
        {
            return IsOutOfRange(newRange.Min, newRange.Max);
        }
    }

 


}