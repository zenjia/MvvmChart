using System.Windows;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// A handy class to draw step poly line series.
    /// </summary>
    public class StepLineSeries : PathSeries
    {
        static StepLineSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StepLineSeries), new FrameworkPropertyMetadata(typeof(StepLineSeries)));
        }


    }
}