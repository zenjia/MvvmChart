using System.Windows;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// A handy class to draw step poly line series.
    /// </summary>
    public class StepLineAreaSeries : PathSeries
    {
        static StepLineAreaSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StepLineAreaSeries), new FrameworkPropertyMetadata(typeof(StepLineAreaSeries)));
        }



    }
}