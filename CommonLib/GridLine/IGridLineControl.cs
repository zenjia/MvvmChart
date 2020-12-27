using System.Collections.Generic;
 
using MvvmCharting.Axis;

namespace MvvmCharting.GridLine
{
    public interface IGridLineControl
    {
        void OnAxisItemCoordinateChanged(AxisType orientation, IEnumerable<double> ticks);
    }
}