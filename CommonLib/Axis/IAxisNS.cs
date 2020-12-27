using System;
using System.Collections.Generic;
using MvvmCharting.Common;
using MvvmCharting.Axis;
using MvvmCharting.Axis;


namespace MvvmCharting.Axis
{
    public interface IAxisNS
    {
        IAxisOwner Owner { get; set; }
        AxisPlacement Placement { get; set; }

 
        AxisType Orientation { get; set; }

        event Action<IAxisNS> AxisPlacementChanged;
        IEnumerable<double> GetAxisItemCoordinates();

        int TickCount { get; set; }


        /// <summary>
        /// User provided converters for convert value to the LabelText of an <see cref="IAxisItem"/>.
        /// </summary>
        IValueConverterNS LabelTextConverter { get; set; }
    }
}
