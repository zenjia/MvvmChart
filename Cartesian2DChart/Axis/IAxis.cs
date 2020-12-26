using System;
using System.Collections.Generic;
using System.Windows.Controls;
using MvvmCharting.Axis;


namespace Cartesian2DChart.Axis
{
    public interface IAxis
    {
        IAxisOwner Owner { get; set; }
        AxisPlacement Placement { get; set; }
        Orientation? Orientation { get; set; }

        event Action<IAxis> AxisPlacementChanged;
        IEnumerable<double> GetAxisItemCoordinates();
    }
}
