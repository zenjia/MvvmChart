using System;
using MvvmChart.Common;

namespace MvvmCharting
{
    /// <summary>
    /// This is the interface that a series should be implement before
    /// it can be used as the root element of the SeriesTemplate of a <see cref="SeriesChart"/>.
    /// </summary>
    public interface ISeries
    {
        /// <summary>
        /// This is used to let its host chart see its X value range
        /// </summary>
        Range XDataRange { get; }

        /// <summary>
        /// This is used to let its host chart see its Y value range
        /// </summary>
        Range YDataRange { get; }

        /// <summary>
        /// The X value range which is used to plot the whole chart.
        /// A series need to know the plotting X range of its host chart
        /// </summary>
        Range PlottingXDataRange { get; set; }

        /// <summary>
        /// The Y value range which is used to plot the whole chart.
        /// A series need to know the plotting Y range of its host chart
        /// </summary>
        Range PlottingYDataRange { get; set; }

        /// <summary>
        /// Fired when the X value range of a series is changed. 
        /// The host chart need to know when the value range of its series changed
        /// </summary>
        event Action<Range> XRangeChanged;

        /// <summary>
        /// Fired when the Y value range of a series is changed. 
        /// The host chart need to know when the value range of its series changed
        /// </summary>
        event Action<Range> YRangeChanged;
    }
}