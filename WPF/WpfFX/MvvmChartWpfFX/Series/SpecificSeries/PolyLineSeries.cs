using System.Windows;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// A handy class to draw poly line series.
    /// </summary>
    public class PolyLineSeries : PathSeries
    {
        static PolyLineSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PolyLineSeries), new FrameworkPropertyMetadata(typeof(PolyLineSeries)));
        }


    }
}