using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MvvmCharting.Common;

namespace MvvmCharting.Axis
{
    public class CategoryAxisDrawingSettings : ICategoryAxisDrawingSettings
    {
 

        #region input values 

        public IList<object> PlottingItemValues { get;  }

        public double PlottingLength { get; }
        #endregion

        #region generated values

        public double ActualTickInterval { get; }

        public int ActualTickCount { get; }

        #endregion

        public CategoryAxisDrawingSettings(int tickInterval, IList<object> plottingItemValues, double plottingLength)
        {
            if (plottingItemValues == null)
            {
                return;
            }
             
            this.PlottingItemValues = plottingItemValues;


            this.PlottingLength = plottingLength;

            //calculated state:
            this.ActualTickInterval = tickInterval;

            this.ActualTickCount = (int)Math.Floor(plottingItemValues.Count / this.ActualTickInterval) + 1;
        }


        public override bool Equals(object obj)
        {
            var other = obj as CategoryAxisDrawingSettings;
            if (other == null)
            {
                return false;
            }

            return this.PlottingLength.NearlyEqual(other.PlottingLength, 0.0001) &&
                   this.PlottingItemValues?.Count == other.PlottingItemValues?.Count &&
                   (this.PlottingItemValues != null && this.PlottingItemValues.SequenceEqual(other.PlottingItemValues)) &&
                   this.ActualTickCount == other.ActualTickCount &&
                   this.ActualTickInterval == other.ActualTickInterval;
        }

        public bool CanUpdateAxisItems()
        {

            return !this.PlottingLength.IsNaNOrZero() && !this.ActualTickInterval.IsNaNOrZero();
        }

        public bool CanUpdateAxisItemsCoordinate()
        {
            return !this.PlottingLength.IsNaNOrZero() && this.PlottingItemValues?.Count > 0;
        }

    }
}