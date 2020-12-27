using MvvmCharting.Common;
using MvvmCharting.Drawing;

namespace MvvmCharting.Series
{
    public interface IScatter: IPlottable_2D
    {
   
        PointNS GetOffsetForSizeChangedOverride();

        object DataContext { get; }
    }
}