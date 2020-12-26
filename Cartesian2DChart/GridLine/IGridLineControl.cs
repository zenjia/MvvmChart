using System.Collections.Generic;
using System.Windows.Controls;
using MvvmChart.Common.Axis;

namespace MvvmCharting
{
    public interface IGridLineControl
    {
        void OnAxisItemCoordinateChanged(AxisType orientation, IEnumerable<double> ticks);
    }
}