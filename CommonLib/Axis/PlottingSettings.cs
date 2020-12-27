 
using MvvmCharting.Common;
 
using MvvmCharting.Drawing;
using MvvmCharting.Drawing;

namespace MvvmCharting.Axis
{
    public interface IPlottingSettingsBase
    {
        AxisType Orientation { get; }
        double RenderSize { get; }

        PointNS Margin { get; }
        PointNS Padding { get; }
        PointNS BorderThickness { get; }

        double GetAvailablePlottingSize();
    }

    public interface ILinearPlottingSettings: IPlottingSettingsBase
    {
        Range PlottingDataRange { get; }
    }

    public class PlottingSettingsBase : IPlottingSettingsBase
    {

        public AxisType Orientation { get; }
        public double RenderSize { get; }

        public PointNS Margin { get; }
        public PointNS Padding { get; }
        public PointNS BorderThickness { get; }

        //public Range PlottingDataRange { get; }

        public double GetAvailablePlottingSize()
        {
            return this.RenderSize - (this.Margin.X + this.Margin.Y)
                                   - (this.Padding.X + this.Padding.Y) -
                                   (this.BorderThickness.X + this.BorderThickness.Y);
        }

        public PlottingSettingsBase(AxisType orientation,
            double renderSize,
            PointNS margin,
            PointNS padding,
            PointNS borderThickness/*,
            Range plottingDataRange*/)
        {
            this.RenderSize = renderSize;
            this.Margin = margin;
            this.Padding = padding;
            this.BorderThickness = borderThickness;
            //this.PlottingDataRange = plottingDataRange;
            this.Orientation = orientation;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PlottingSettingsBase;
            if (other == null)
            {
                return false;
            }
            return other.Equals(this);
        }

        public bool Equals(PlottingSettingsBase obj)
        {
            return this.RenderSize.NearlyEqual(obj.RenderSize, 0.0001) &&
                   this.Margin == obj.Margin &&
                   this.Padding == obj.Padding &&
                   this.BorderThickness == obj.BorderThickness 
                   //&&
                   //this.PlottingDataRange == obj.PlottingDataRange
                ;
        }

        public static bool Validate(double length,
            PointNS margin,
            PointNS pading,
            PointNS borderThickness/*,
            Range plotingDataRange*/)
        {
            return !length.IsInvalid() &&
                   !length.IsZero() &&
                   !margin.IsInvalid() &&
                   !pading.IsInvalid() &&
                   !borderThickness.IsInvalid()/* &&
                   !plotingDataRange.IsInvalid*/;

        }
    }

    public class PlottingSettings: PlottingSettingsBase, ILinearPlottingSettings
    {
        
 

        public Range PlottingDataRange { get; }

 

        public PlottingSettings(AxisType orientation,
            double renderSize,
            PointNS margin,
            PointNS padding,
            PointNS borderThickness,
            Range plottingDataRange)
                :base(orientation, renderSize, margin, padding, borderThickness)
        {
 
            this.PlottingDataRange = plottingDataRange;
 
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
            var plottingSettingsBase = obj as PlottingSettingsBase;
            return plottingSettingsBase.Equals(this) &&
                   this.PlottingDataRange == obj.PlottingDataRange;
        }

        public static bool Validate(double length,
            PointNS margin,
            PointNS pading,
            PointNS borderThickness,
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