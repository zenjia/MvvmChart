using System;
using System.Collections.Generic;
using MvvmChart.Common.Axis;
using MvvmCharting.Axis;


namespace Cartesian2DChart.Axis
{
    public interface IAxis
    {
        IAxisOwner Owner { get; set; }
        AxisPlacement Placement { get; set; }

 
        AxisType Orientation { get; set; }

        event Action<IAxis> AxisPlacementChanged;
        IEnumerable<double> GetAxisItemCoordinates();
    }
}
