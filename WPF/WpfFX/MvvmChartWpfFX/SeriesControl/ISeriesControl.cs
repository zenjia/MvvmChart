using System;
using System.Windows;
using MvvmCharting.Common;

namespace MvvmCharting.WpfFX.Series
{
    /// <summary>
    /// This is the interface that a series should be implement before
    /// it can be used as the root element of SeriesTemplate
    /// </summary>
    public interface ISeriesControl
    {
        Range2D ValueRange { get; }

        ///// <summary>
        ///// This is used to let its host chart see its X value range
        ///// </summary>
        //Range XValueRange { get; }

        ///// <summary>
        ///// This is used to let its host chart see its Y value range
        ///// </summary>
        //Range YValueRange { get; }

        void UpdateValueRange();

 

        bool IsHighlighted { get; set; }

        object DataContext { get; }

        Point[] GetCoordinates();

        /// <summary>
        /// Fired when the X value range of a series is changed. 
        /// The host chart need to know when the value range of its series changed
        /// </summary>
        event Action<ISeriesControl, Range> XRangeChanged;

        /// <summary>
        /// Fired when the Y value range of a series is changed. 
        /// The host chart need to know when the value range of its series changed
        /// </summary>
        event Action<ISeriesControl, Range> YRangeChanged;

        //event Action<object, string> PropertyChanged;
    }
}