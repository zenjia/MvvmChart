using System.Windows;

namespace MvvmCharting
{
    public interface IScatter: IPlottable_2D
    {
   
        Point GetOffsetForSizeChangedOverride(Size newSize);

        object DataContext { get; }
    }
}