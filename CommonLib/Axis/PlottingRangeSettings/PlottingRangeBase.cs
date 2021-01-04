using MvvmCharting.Common;

namespace MvvmCharting.Axis
{
    public class PlottingRangeBase
    {
        public Range ValuePadding { get; }

        protected PlottingRangeBase(Range valuePadding)
        {
            this.ValuePadding = valuePadding;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PlottingRangeBase);
        }

        public virtual bool Equals(PlottingRangeBase obj)
        {
            if (obj == null)
            {
                return false;
            }

            return this.ValuePadding == obj.ValuePadding;
        }

        public static bool operator ==(PlottingRangeBase rangeSetting, PlottingRangeBase settings2)
        {
            if (object.ReferenceEquals(rangeSetting, null))
            {
                return object.ReferenceEquals(settings2, null);
            }

            return rangeSetting.Equals(settings2);
        }

        public static bool operator !=(PlottingRangeBase rangeSetting, PlottingRangeBase settings2)
        {
            return !(rangeSetting == settings2);
        }
    }
}