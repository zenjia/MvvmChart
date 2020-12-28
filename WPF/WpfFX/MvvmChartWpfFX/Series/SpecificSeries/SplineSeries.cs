using System.Windows;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// A handy class to draw spline series.
    /// </summary>
    public class SplineSeries : PathSeries
    {
        static SplineSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplineSeries), new FrameworkPropertyMetadata(typeof(SplineSeries)));
        }

    }
}