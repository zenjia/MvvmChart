using System.Collections.Generic;
using System.Windows.Controls;
using MvvmChart.Common.Axis;

namespace MvvmCharting.Axis
{
    public interface IAxisOwner
    {
        void OnAxisItemsCoordinateChanged(AxisType orientation, IEnumerable<double> ticks);
    }
}