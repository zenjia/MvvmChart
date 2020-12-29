using System.Collections;
using MvvmCharting.Drawing;

namespace MvvmCharting.WpfFX
{
    public interface ISeriesHost
    {
        IList SeriesItemsSource { get; }
        PointNS[] GetPreviousSeriesCoordinates();
    }
}