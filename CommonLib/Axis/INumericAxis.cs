using System;
using System.Collections.Generic;
 

namespace MvvmCharting.Axis
{
    /// <summary>
    /// Represents a linear axis for numerical(which implements <see cref="IConvertible"/>) value. 
    /// </summary>
    public interface INumericAxis: IAxisNS
    {
        double TickInterval { get; }
        IList<double> ExplicitTicks { get; }
    }
}