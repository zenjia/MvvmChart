namespace MvvmCharting.Common
{
    /// <summary>
    /// Represents a UIElement that can be plotted(positioned) in a
    /// 1-dimension space
    /// </summary>
    public interface IPlottable_1D
    {
        double Coordinate { get; set; }
    }
}