using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MvvmCharting.Common;

namespace MvvmCharting.Axis
{
    public class CategoryAxisDrawingSettings : AxisDrawingSettingsBase
    {


        private IList<object> PlottingItemValues { get; }

        private Range ValuePadding { get; }

        public CategoryAxisDrawingSettings(int tickInterval, CategoryPlottingRange plottingRange, double plottingLength)
            : base(plottingLength)
        {
            if (plottingRange == null)
            {
                throw new ArgumentException();
            }

            var list = plottingRange.PlottingItemValues;
            if (list == null || list.Count == 0)
            {
                throw new ArgumentException();
            }

            this.PlottingItemValues = plottingRange.PlottingItemValues;
            this.ValuePadding = plottingRange.ValuePadding;
             
       

            this.PixelPerUnit = this.PlottingLength / (this.PlottingItemValues.Count + this.ValuePadding.Min + this.ValuePadding.Max);


            //calculated state:
            this.ActualTickInterval = tickInterval;

            this.ActualTickCount = (int)Math.Floor(this.PlottingItemValues.Count / this.ActualTickInterval) + 1;


        }




        public override bool Equals(AxisDrawingSettingsBase obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            var other = obj as CategoryAxisDrawingSettings;
            if (other == null)
            {
                return false;
            }

            return this.PlottingItemValues?.Count == other.PlottingItemValues?.Count &&
                   this.PlottingItemValues != null && 
                   this.PlottingItemValues.SequenceEqual(other.PlottingItemValues);
        }

        public override IEnumerable<object> GetPlottingValues()
        {
            return this.PlottingItemValues;
        }

        public override bool CanUpdateAxisItemsCoordinate()
        {
            return !this.PlottingLength.IsNaNOrZero() && this.PlottingItemValues?.Count > 0;
        }

        public override double CalculateCoordinate(double i, AxisType axisType)
        {

            var coordinate = (i + this.ValuePadding.Min) * this.PixelPerUnit;

            if (axisType == AxisType.Y)
            {
                coordinate = this.PlottingLength - coordinate;
            }

            return coordinate;
        }

    }
}