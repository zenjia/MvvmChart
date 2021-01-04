using System;
using System.Collections.Generic;
using MvvmCharting.Common;

namespace MvvmCharting.Axis
{
    public abstract class AxisDrawingSettingsBase : IAxisDrawingSettingsBase
    {
        protected double PlottingLength { get; }
        protected double ActualTickInterval { get; set; }
        protected int ActualTickCount { get; set; }
        protected double PixelPerUnit { get; set; }

        protected AxisDrawingSettingsBase(double plottingLength)
        {
            this.PlottingLength = plottingLength;
        }

        public abstract IEnumerable<object> GetPlottingValues();
 

        public virtual bool CanUpdateAxisItems()
        {
            return !this.PlottingLength.IsNaNOrZero() && !this.ActualTickInterval.IsNaNOrZero();
        }

        public virtual bool CanUpdateAxisItemsCoordinate()
        {
            return true;
        }

        public abstract double CalculateCoordinate(double value, AxisType axisType);


        public override bool Equals(object obj)
        {
            return Equals(obj as AxisDrawingSettingsBase);
        }

        public virtual bool Equals(AxisDrawingSettingsBase other)
        {
 
            if (other == null)
            {
                return false;
            }

            return this.PlottingLength.NearlyEqual(other.PlottingLength, 0.0001) &&
                   this.ActualTickCount == other.ActualTickCount &&
                   this.ActualTickInterval == other.ActualTickInterval &&
                   this.PixelPerUnit == other.PixelPerUnit;
        }
    }
}