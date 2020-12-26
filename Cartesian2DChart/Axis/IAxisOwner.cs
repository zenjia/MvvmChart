using System.Collections.Generic;
using System.Windows.Controls;

namespace MvvmCharting.Axis
{
    public interface IAxisOwner
    {
        void OnAxisItemsCoordinateChanged(Orientation orientation, IEnumerable<double> ticks);
    }
}