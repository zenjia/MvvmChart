using System;
using System.Collections.Generic;
using MvvmCharting.Common;

namespace MvvmCharting.Axis
{
 



    public class NumericAxisDrawingSettings : AxisDrawingSettingsBase
    {
 
 
        private Range FullPlottingRange { get; }

        private Range ActualPlottingRange { get; }
        
        public NumericAxisDrawingSettings(int tickCount, double tickInterval,
            NumericPlottingRange plottingRange, double plottingLength)
            : base(plottingLength)
        {
            var range = plottingRange.ActualRange;
            if (range.IsInvalid || plottingLength.IsNaNOrZero())
            {
                throw new ArgumentException();
            }

            if (tickCount < 0 || tickCount > ushort.MaxValue)
            {
                throw new NotSupportedException();
            }


            this.ActualPlottingRange = plottingRange.ActualRange;
            this.FullPlottingRange = plottingRange.FullRange;

            this.PixelPerUnit = plottingLength / plottingRange.FullRange.Span;

            //calculated state:
            this.ActualTickInterval = !tickInterval.IsNaN() ? tickInterval : this.ActualPlottingRange.Span / tickCount;
            this.ActualTickCount = (int)Math.Floor(this.ActualPlottingRange.Span / this.ActualTickInterval) + 1;
        }


        public override bool Equals(AxisDrawingSettingsBase obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }
            var other = obj as NumericAxisDrawingSettings;
            if (other == null)
            {
                return false;
            }

            return this.FullPlottingRange == other.FullPlottingRange &&
                   this.ActualPlottingRange == other.ActualPlottingRange;
        }

 

        public override bool CanUpdateAxisItemsCoordinate()
        {
            return !this.PlottingLength.IsNaNOrZero() && !this.FullPlottingRange.Span.IsNaNOrZero();
        }
        public override IEnumerable<object> GetPlottingValues()
        {
            var startValue = this.ActualPlottingRange.Min;


            for (int i = 0; i < this.ActualTickCount; i++)
            {

                yield return startValue + i * this.ActualTickInterval;
            }
        }

        public override double CalculateCoordinate(double value, AxisType axisType)
        {
            var offset = (value - this.FullPlottingRange.Min) * this.PixelPerUnit;

            if (axisType == AxisType.Y)
            {
                offset = this.PlottingLength - offset;
            }

            return offset;
        }

    }
}