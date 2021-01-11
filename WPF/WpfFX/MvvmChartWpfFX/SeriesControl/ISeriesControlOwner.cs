using System.Collections.Generic;
using MvvmCharting.Common;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX.Series
{
    public interface ISeriesControlOwner
    {
        StackMode StackMode { get; }
        IEnumerable<SeriesControl> GetSeries();
         
        ISeriesControl GetPreviousSeriesHost(ISeriesControl current);
        

        bool IsSeriesCollectionChanging { get; }
        bool IsXAxisCategory { get;}

 
        double XStartValue { get; }
        double YStartValue { get; }
        double XPixelPerUnit { get; }
        double YPixelPerUnit { get; }

        double YBaseValue { get; }
        double YBaseCoordinate { get; }
    }
}