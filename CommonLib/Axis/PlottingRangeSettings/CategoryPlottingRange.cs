using System.Collections.Generic;
using System.Linq;
using MvvmCharting.Common;

namespace MvvmCharting.Axis
{


    public class CategoryPlottingRange : PlottingRangeBase
    {
        public CategoryPlottingRange(IList<object> plottingItemValues, Range valuePadding)
        :base(valuePadding)
        {
            this.PlottingItemValues = plottingItemValues;
        }
         
        public IList<object> PlottingItemValues { get; }
         

        public override bool Equals(PlottingRangeBase obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            var other = obj as CategoryPlottingRange;

            return this.ValuePadding == obj.ValuePadding && this.PlottingItemValues.SequenceEqual(other.PlottingItemValues);
        }

       
    }
}