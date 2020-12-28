using System.Windows;

namespace MvvmCharting.WpfFX
{
    public class PolyLineAreaSeries : PathSeries
    {
        static PolyLineAreaSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PolyLineAreaSeries), new FrameworkPropertyMetadata(typeof(PolyLineAreaSeries)));
        }

    }
}