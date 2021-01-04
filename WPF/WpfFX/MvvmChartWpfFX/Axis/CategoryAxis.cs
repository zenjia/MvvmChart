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
    /// Only x-axis(independent axis) can be CategoryAxis.
    /// </summary>
    public class CategoryAxis : AxisBase, ICategoryAxis
    {
        static CategoryAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CategoryAxis), new FrameworkPropertyMetadata(typeof(CategoryAxis)));
        }

        protected override void UpdateAxisDrawingSettings()
        {
            if (this.PlottingRangeSetting == null)
            {
                return;
            }

            var length = this.Orientation == AxisType.X ? this.ActualWidth : this.ActualHeight;
            var categoryPlottingSettings = (CategoryPlottingRange)this.PlottingRangeSetting;
            var axisDrawingSettings = new CategoryAxisDrawingSettings(this.TickCount, categoryPlottingSettings, length);
            this.DrawingSettings = axisDrawingSettings;
        }

        public override IEnumerable<double> GetAxisItemCoordinates()
        {
            if (this.PlottingRangeSetting == null)
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

            var dataValues = drawingSettings.GetPlottingValues().ToArray();

            DoUpdateAxisItems(dataValues);

            return true;
        }

        protected override void DoUpdateAxisItemsCoordinate()
        {

            int i = 0;
            foreach (IAxisItem item in this.GetAllAxisItems())
            {
                var coordinate = this.DrawingSettings.CalculateCoordinate(i + 0.5, this.Orientation);

                item.Coordinate = coordinate;

                i++;

            }
        }
    }
}