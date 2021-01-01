 
using MvvmCharting.Common;

namespace MvvmCharting.Axis
{
    public interface IAxisItem: IPlottable_1D
    {
        object DataContext { get; }

    }
}