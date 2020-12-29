using System.Collections;
using System.Collections.Generic;
using MvvmCharting.Drawing;

namespace MvvmCharting.WpfFX
{
    public interface ISeriesHost
    {
 
        IList SeriesItemsSource { get; }
        IEnumerable<SeriesBase> GetSeries();
    }
}