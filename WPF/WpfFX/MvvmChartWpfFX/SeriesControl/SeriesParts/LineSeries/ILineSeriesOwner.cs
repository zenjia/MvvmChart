using System.Collections;
using System.Windows;

namespace MvvmCharting.WpfFX.Series
{
    public interface ILineSeriesOwner
    {
        ISeriesControlOwner SeriesControlOwner { get; }
        IList ItemsSource { get; }

        double XPixelPerUnit { get; }
        double YPixelPerUnit { get; }

        Point[] GetCoordinates();

        Point[] GetPreviousSeriesCoordinates(bool isAreaSeries);

    }
}