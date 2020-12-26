using System;
using System.Diagnostics;
using MvvmChart.Common;

namespace MvvmCharting.Axis
{
    internal class AxisDrawingSettings
    {
        public override string ToString()
        {
            return string.Format($"Input values: PlotingDataRange={this.PlotingDataRange}, PlotingDataRange.Span={PlotingDataRange.Span}, PlotingLength={this.PlotingLength}" + Environment.NewLine
                                                                                                                                                                        + $"Calculated values: ActualTickInterval={ActualTickInterval}, ActualTickCount={ActualTickCount}");
        }

        #region input values
        public Range PlotingDataRange { get; }

        public double PlotingLength { get; }
        #endregion

        #region generated values

        public double ActualTickInterval { get; } 

        public int ActualTickCount { get; }

        #endregion

        public AxisDrawingSettings(int tickCount, double tickInterval, Range range, double plotingLength)
        {

            if (tickCount<0 || tickCount>ushort.MaxValue)
            {
                throw new NotSupportedException();
            }

 

            this.PlotingDataRange = range;

 
            this.PlotingLength = plotingLength;

            //calculated state:
            this.ActualTickInterval = !tickInterval.IsNaN() ? tickInterval : this.PlotingDataRange.Span / tickCount;
            this.ActualTickCount = (int)Math.Floor(this.PlotingDataRange.Span / this.ActualTickInterval) + 1;
        }


        public override bool Equals(object obj)
        {
            var other = obj as AxisDrawingSettings;
            if (other == null)
            {
                return false;
            }

            return this.PlotingLength.NearlyEqual(other.PlotingLength, 0.0001) && 
                   this.PlotingDataRange==other.PlotingDataRange &&
                   this.ActualTickCount == other.ActualTickCount &&
                   this.ActualTickInterval == other.ActualTickInterval;
        }

        public bool CanUpdateAxisItems()
        {
 
            return !this.PlotingLength.IsNaNOrZero() && !this.ActualTickInterval.IsNaNOrZero();
        }

        public bool CanUpdateAxisItemsCoordinate()
        {
            return !this.PlotingLength.IsNaNOrZero() && !this.PlotingDataRange.Span.IsNaNOrZero();
        }
 
    }
}