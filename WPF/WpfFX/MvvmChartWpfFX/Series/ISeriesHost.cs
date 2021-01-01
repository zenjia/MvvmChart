using System.Collections.Generic;

namespace MvvmCharting.WpfFX
{
    public interface ISeriesHost
    {
        IEnumerable<SeriesBase> GetSeries();
        int SeriesCount { get; }

        bool IsSeriesCollectionChanging { get; }
        bool IsXAxisCategory { get;}
    }
}