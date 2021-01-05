using System.Collections;
using MvvmCharting.Drawing;

namespace MvvmCharting.WpfFX.Series
{
    public interface ILineSeriesOwner
    {
        ISeriesControlOwner SeriesControlOwner { get; }
        IList ItemsSource { get; }

        double XPixelPerUnit { get; }
        double YPixelPerUnit { get; }

        PointNS[] GetCoordinates();

        PointNS[] GetPreviousSeriesCoordinates(bool isAreaSeries);

    }
}