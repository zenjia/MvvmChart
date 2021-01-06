using System.Windows;
using MvvmCharting.Common;

namespace MvvmCharting.WpfFX
{
    public static class PlottingRangeHelper
    {
        public static PlottingRange PlottingRange(Range actualRange, Point padding)
        {
            return new PlottingRange(actualRange, padding.X, padding.Y);
        }
    }
}