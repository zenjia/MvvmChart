using MvvmCharting.Common;

namespace MvvmCharting.Axis
{

    public class NumericPlottingRange : PlottingRangeBase
    {
        public Range ActualRange { get; }
        public Range FullRange
        {
            get
            {
                return new Range(this.ActualRange.Min - this.ValuePadding.Min,
                    this.ActualRange.Max + this.ValuePadding.Max);
            }
        }


        public NumericPlottingRange(Range actualRange, Range valuePadding)
          :base(valuePadding)
        {
            this.ActualRange = actualRange;
 
        }



        public override bool Equals(PlottingRangeBase obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            var other = obj as NumericPlottingRange;
            if (other == null)
            {
                return false;
            }
            return this.ActualRange == other.ActualRange;
        }

        
    }
}