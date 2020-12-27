using System.Collections.Generic;

namespace MvvmCharting.Axis
{
    public interface ICategoryAxisOwner
    {
        IEnumerable<object> GetCategoryValuesForAxisPlotting();
    }
}