using System.Windows;
using System.Windows.Controls;
using MvvmChart.Common;

namespace MvvmCharting.Axis
{
    public class PlottingSettings
    {
 

        public Orientation Orientation { get; }
        public double RenderSize { get; }

        public Point Margin { get; }
        public Point Padding { get; }
        public Point BorderThickness { get; }

        public Range PlotingDataRange { get; }

        public double GetAvailablePlottingSize()
        {
            return this.RenderSize - (this.Margin.X + this.Margin.Y)
                                   - (this.Padding.X + this.Padding.Y) -
                                   (this.BorderThickness.X + this.BorderThickness.Y);
        }

        public PlottingSettings(Orientation orientation,
            double renderSize,
            Point margin,
            Point padding,
            Point borderThickness,
            Range plotingDataRange)
        {
            this.RenderSize = renderSize;
            this.Margin = margin;
            this.Padding = padding;
            this.BorderThickness = borderThickness;
            this.PlotingDataRange = plotingDataRange;
            this.Orientation = orientation;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PlottingSettings;
            if (other == null)
            {
                return false;
            }
            return other.Equals(this);
        }

        public bool Equals(PlottingSettings obj)
        {
            return this.RenderSize.NearlyEqual(obj.RenderSize, 0.0001) &&
                   this.Margin == obj.Margin &&
                   this.Padding == obj.Padding &&
                   this.BorderThickness == obj.BorderThickness &&
                   this.PlotingDataRange == obj.PlotingDataRange;
        }

        public static bool Validate(double length,
            Point margin,
            Point pading,
            Point borderThickness,
            Range plotingDataRange)
        {
            return !length.IsInvalid() &&
                   !length.IsZero() &&
                   !margin.IsInvalid() &&
                   !pading.IsInvalid() &&
                   !borderThickness.IsInvalid() &&
                   !plotingDataRange.IsInvalid;

        }
    }
}