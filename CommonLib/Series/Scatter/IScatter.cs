using MvvmChart.Common.Drawing;

namespace MvvmCharting
{
    public interface IScatter: IPlottable_2D
    {
   
        PointNS GetOffsetForSizeChangedOverride();

        object DataContext { get; }
    }
}