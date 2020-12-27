using MvvmCharting.Drawing;


namespace MvvmCharting.Common
{    
    /// <summary>
    /// Represents a UIElement that can be plotted(positioned) in a
    /// 2-dimension space
    /// </summary>
    public interface IPlottable_2D
    {

        PointNS Coordinate { get; set; }
    }
}