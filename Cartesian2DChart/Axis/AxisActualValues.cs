using System;
using System.Diagnostics;
using MvvmChart.Common;

namespace MvvmCharting.Axis
{
    internal class AxisActualValues
    {
        public override string ToString()
        {
            return string.Format($"Input values: CurrentRange={CurrentRange}, ActualRangeSpan={ActualRangeSpan}, ActualLength={ActualLength}" + Environment.NewLine
            + $"Calculated values: ActualTickInterval={ActualTickInterval}, ActualTickCount={ActualTickCount}");
        }

        #region input values

        //private readonly int _tickCount;
        //private readonly double _tickInterval;

        public Range CurrentRange { get; }

        public double ActualRangeSpan { get; }  

        public double ActualLength { get; }  
        
        #endregion

        #region generated values

        public double ActualTickInterval { get; } 

        public int ActualTickCount { get; }

        #endregion

        public AxisActualValues(int tickCount, double tickInterval, Range range, double actualLength)
        {
            //this._tickCount = tickCount;
            //this._tickInterval = tickInterval;

            if (tickCount<0 || tickCount>ushort.MaxValue)
            {
                throw new NotSupportedException();
            }

            if (!range.IsValid)
            {
                throw new NotSupportedException();
            }

            this.CurrentRange = range;

 
            this.ActualLength = actualLength;

            //below is calculated state:
            this.ActualRangeSpan = range.Span;
            this.ActualTickInterval = !tickInterval.IsNaN() ? tickInterval : this.ActualRangeSpan / tickCount;
            this.ActualTickCount = (int)Math.Floor(this.ActualRangeSpan / this.ActualTickInterval) + 1;
        }


        public override bool Equals(object obj)
        {
            var other = obj as AxisActualValues;
            if (other == null)
            {
                return false;
            }

            return this.ActualLength==other.ActualLength && 
                   this.CurrentRange==other.CurrentRange &&
                   this.ActualTickCount == other.ActualTickCount &&
                   this.ActualTickInterval == other.ActualTickInterval;
        }

        public bool CanUpdateAxisItems()
        {
 
            return !this.ActualLength.IsNaNOrZero() && !this.ActualTickInterval.IsNaNOrZero();
        }

        public bool CanUpdateAxisItemsOffset()
        {
            return !this.ActualLength.IsNaNOrZero() && !this.ActualRangeSpan.IsNaNOrZero();
        }
 
    }
}