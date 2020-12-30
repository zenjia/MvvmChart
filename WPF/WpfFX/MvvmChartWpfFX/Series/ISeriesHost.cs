using System.Collections.Generic;

namespace MvvmCharting.WpfFX
{
    public interface ISeriesHost
    {
        IEnumerable<SeriesBase> GetSeries();
    }
}