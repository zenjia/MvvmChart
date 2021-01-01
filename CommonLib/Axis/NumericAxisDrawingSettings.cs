using System;
using System.Collections.Generic;
using MvvmCharting.Common;

namespace MvvmCharting.Axis
{
    public interface IAxisDrawingSettingsBase
    {
        double PlottingLength { get; }

        double ActualTickInterval { get; }

        int ActualTickCount { get; }

        bool CanUpdateAxisItems();

        bool CanUpdateAxisItemsCoordinate();
    }

    public interface INumericAxisDrawingSettings : IAxisDrawingSettingsBase
    {
        Range PlottingDataRange { get; }
    }

    public interface ICategoryAxisDrawingSettings : IAxisDrawingSettingsBase
    {
        IList<object> PlottingItemValues { get; }
    }


    public class NumericAxisDrawingSettings : INumericAxisDrawingSettings
    {
        public override string ToString()
        {
            return string.Format($"Input values: PlottingDataRange={this.PlottingDataRange}, PlottingDataRange.Span={this.PlottingDataRange.Span}, PlottingLength={this.PlottingLength}" + Environment.NewLine
                                                                                                                                                                                         + $"Calculated values: ActualTickInterval={ActualTickInterval}, ActualTickCount={ActualTickCount}");
        }

        #region input values
        public Range PlottingDataRange { get; }

        public double PlottingLength { get; }
        #endregion

        #region generated values

        public double ActualTickInterval { get; }

        public int ActualTickCount { get; }

        #endregion

        public NumericAxisDrawingSettings(int tickCount, double tickInterval, Range range, double plottingLength)
        {

            if (tickCount < 0 || tickCount > ushort.MaxValue)
            {
                throw new NotSupportedException();
            }



            this.PlottingDataRange = range;


            this.PlottingLength = plottingLength;

            //calculated state:
            this.ActualTickInterval = !tickInterval.IsNaN() ? tickInterval : this.PlottingDataRange.Span / tickCount;
            this.ActualTickCount = (int)Math.Floor(this.PlottingDataRange.Span / this.ActualTickInterval) + 1;
        }


        public override bool Equals(object obj)
        {
            var other = obj as NumericAxisDrawingSettings;
            if (other == null)
            {
                return false;
            }

            return this.PlottingLength.NearlyEqual(other.PlottingLength, 0.0001) &&
                   this.PlottingDataRange == other.PlottingDataRange &&
                   this.ActualTickCount == other.ActualTickCount &&
                   this.ActualTickInterval == other.ActualTickInterval;
        }

        public bool CanUpdateAxisItems()
        {

            return !this.PlottingLength.IsNaNOrZero() && !this.ActualTickInterval.IsNaNOrZero();
        }

        public bool CanUpdateAxisItemsCoordinate()
        {
            return !this.PlottingLength.IsNaNOrZero() && !this.PlottingDataRange.Span.IsNaNOrZero();
        }

    }
}