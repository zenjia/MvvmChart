using System.Collections.Generic;
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
    }
}