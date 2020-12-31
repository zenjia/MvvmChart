using System;
using MvvmCharting.Common;

namespace MvvmCharting.Series
{
    /// <summary>
    /// This is the interface that a series should be implement before
    /// it can be used as the root element of SeriesTemplate
    /// </summary>
    public interface ISeries
    {
        /// <summary>
        /// This is used to let its host chart see its X value range
        /// </summary>
        Range XValueRange { get; }

        /// <summary>
        /// This is used to let its host chart see its Y value range
        /// </summary>
        Range YValueRange { get; }

        void UpdateValueRange();

        /// <summary>
        /// The X value range which is used to plot the whole chart.
        /// A series need to know the plotting X range of its host chart
        /// </summary>
        void OnPlottingXValueRangeChanged(Range newValue);

        /// <summary>
        /// The Y value range which is used to plot the whole chart.
        /// A series need to know the plotting Y range of its host chart
        /// </summary>
        void OnPlottingYValueRangeChanged(Range newValue);


        bool IsHighLighted { get; set; }

        object DataContext { get; }

        

        /// <summary>
        /// Fired when the X value range of a series is changed. 
        /// The host chart need to know when the value range of its series changed
        /// </summary>
        event Action<ISeries, Range> XRangeChanged;

        /// <summary>
        /// Fired when the Y value range of a series is changed. 
        /// The host chart need to know when the value range of its series changed
        /// </summary>
        event Action<ISeries, Range> YRangeChanged;

        //event Action<object, string> PropertyChanged;
    }
}