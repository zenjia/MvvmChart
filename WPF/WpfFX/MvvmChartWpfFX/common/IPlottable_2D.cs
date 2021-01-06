using System.Windows;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// Represents a UIElement that can be plotted(positioned) in a
    /// 2-dimension space
    /// </summary>
    public interface IPlottable_2D
    {

        Point Coordinate { get; set; }
    }
}