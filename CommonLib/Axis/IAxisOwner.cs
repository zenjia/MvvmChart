using System.Collections.Generic;
 
 

namespace MvvmCharting.Axis
{
    public interface IAxisOwner
    {
        void OnAxisItemsCoordinateChanged(AxisType orientation, IEnumerable<double> ticks);
    }
}