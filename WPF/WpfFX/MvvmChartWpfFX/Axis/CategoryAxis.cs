using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using MvvmCharting.Axis;
using MvvmCharting.Common;

namespace MvvmCharting.WpfFX.Axis
{
    /// <summary>
    /// Represents a discrete axis for category data type.
    /// </summary>
    public class CategoryAxis : AxisBase, ICategoryAxis
    {
        static CategoryAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CategoryAxis), new FrameworkPropertyMetadata(typeof(CategoryAxis)));
        }

        protected override void UpdateAxisDrawingSettings()
        {
            if (/*!this.IsLoaded ||*/
                this.PlottingSetting == null)
            {
                return;
            }

            var plottingItemValues = ((ICategoryPlottingSettings)this.PlottingSetting).PlottingItemValues;
            var length = this.PlottingSetting.GetAvailablePlottingSize();
            if (plottingItemValues == null || plottingItemValues.Count == 0)
            {
                throw new NotImplementedException();
            }

            var axisDrawingSettings = new CategoryAxisDrawingSettings(this.TickCount, plottingItemValues, length);
            this.DrawingSettings = axisDrawingSettings;
        }

        public override IEnumerable<double> GetAxisItemCoordinates()
        {
            if (this.PlottingSetting == null)
            {
                return null;
            }
 
            var coordinates = this.PART_AxisItemsControl.Children.OfType<AxisItem>()
                .Select(x => x.Coordinate).ToArray();
 
            return coordinates;
        }

        protected override bool LoadAxisItems()
        {
            IAxisDrawingSettingsBase drawingSettings = this.DrawingSettings;
            if (drawingSettings == null || !drawingSettings.CanUpdateAxisItems())
            {
                return false;
            }

            if (drawingSettings.Equals(this._currentDrawingSettings))
            {
                return false;
            }

            this._currentDrawingSettings = drawingSettings;

            IList<object> dataValues = ((ICategoryAxisDrawingSettings) drawingSettings).PlottingItemValues;// GetItemValues(((ICategoryAxisDrawingSettings)drawingSettings).PlottingDataRange.Min, drawingSettings.ActualTickInterval, drawingSettings.ActualTickCount);

            DoUpdateAxisItems(dataValues.ToArray());

            return true;
        }

        protected override void DoUpdateAxisItemsCoordinate()
        {
            var list = ((ICategoryAxisDrawingSettings)this.DrawingSettings).PlottingItemValues;
            var length = this.DrawingSettings.PlottingLength;
            var uLen = length / list.Count;

            int i = 0;
            foreach (IAxisItem item in this.GetAllAxisItems())
            {
                var coordinate = (i + 0.5) * uLen;
                if (this.Orientation == AxisType.Y)
                {
                    coordinate = length - coordinate;
                }

                item.Coordinate = coordinate;

                i++;

            }
        }
    }
}