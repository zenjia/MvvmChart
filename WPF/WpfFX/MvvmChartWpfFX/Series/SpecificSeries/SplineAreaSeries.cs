using System.Windows;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// A handy class to draw a closed spline path.
    /// </summary>
    public class SplineAreaSeries : PathSeries
    {
        static SplineAreaSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplineAreaSeries), new FrameworkPropertyMetadata(typeof(SplineAreaSeries)));
        }

    }
}