using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MvvmCharting.Axis;
using MvvmCharting.Common;

namespace MvvmCharting.WpfFX.Axis
{    
    
    /// <summary>
    /// Represents a numeric, linear axis.
    /// </summary>
    public class NumericAxis : AxisBase, INumericAxis
    {
        static NumericAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericAxis), new FrameworkPropertyMetadata(typeof(NumericAxis)));
        }

        #region TickInterval
        public double TickInterval
        {
            get { return (double)GetValue(TickIntervalProperty); }
            set { SetValue(TickIntervalProperty, value); }
        }
        public static readonly DependencyProperty TickIntervalProperty =
            DependencyProperty.Register("TickInterval", typeof(double), typeof(NumericAxis), new PropertyMetadata(double.NaN, OnTickIntervalPropertyChanged));

        private static void OnTickIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericAxis)d).UpdateAxisDrawingSettings(); ;
        }
        #endregion

        #region ExplicitTicks
        public IList<double> ExplicitTicks
        {
            get { return (IList<double>)GetValue(ExplicitTicksProperty); }
            set { SetValue(ExplicitTicksProperty, value); }
        }
        public static readonly DependencyProperty ExplicitTicksProperty =
            DependencyProperty.Register("ExplicitTicks", typeof(IList<double>), typeof(NumericAxis), new PropertyMetadata(null, OnExplicitTicksPropertyChanged));

        private static void OnExplicitTicksPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericAxis)d).OnExplicitTicksChanged((IList<double>)e.OldValue, (IList<double>)e.NewValue);
        }

        private void OnExplicitTicksChanged(IList<double> oldValue, IList<double> newValue)
        {
            TryLoadAxisItems();
        }
        #endregion

        protected override void UpdateAxisDrawingSettings()
        {
            if (/*!this.IsLoaded ||*/
                this.PlottingSetting == null)
            {
                return;
            }

            var range = ((INumericPlottingSettings) this.PlottingSetting).PlottingDataRange;
            var length = this.PlottingSetting.GetAvailablePlottingSize();
            if (range.IsInvalid || length.IsNaNOrZero())
            {
                throw new NotImplementedException();
            }

            var axisDrawingSettings = new NumericAxisDrawingSettings(this.TickCount, this.TickInterval, range, length);
            this.DrawingSettings = axisDrawingSettings;
        }

 
        protected override void DoUpdateAxisItemsCoordinate()
        {
            var span = ((INumericAxisDrawingSettings) this.DrawingSettings).PlottingDataRange.Span;
            var length = this.DrawingSettings.PlottingLength;
            var uLen = length / span;


            foreach (IAxisItem item in this.GetAllAxisItems())
            {
                var coordinate = ((double)item.DataContext - ((INumericAxisDrawingSettings) this.DrawingSettings).PlottingDataRange.Min) * uLen;
                if (this.Orientation == AxisType.Y)
                {
                    coordinate = length - coordinate;
                }

                item.Coordinate = coordinate;


            }
        }

        private IList<double> GetItemValues(double startValue, double tickInterval, int tickCount)
        {
            var chartRange = ((INumericPlottingSettings) this.PlottingSetting).PlottingDataRange;

            var arr = Enumerable.Range(0, tickCount)
                .Select(i => startValue + i * tickInterval)
                .Where(x => chartRange.IsInRange(x))
                .ToArray();

            return arr;
        }

        protected override bool LoadAxisItems()
        {

            IList<double> dataValues;
            if (this.ExplicitTicks != null)
            {
                dataValues = this.ExplicitTicks;
                this._currentDrawingSettings = null;
            }
            else
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

                dataValues = GetItemValues(((INumericAxisDrawingSettings)drawingSettings).PlottingDataRange.Min, drawingSettings.ActualTickInterval, drawingSettings.ActualTickCount);


            }

            DoUpdateAxisItems(dataValues.Select(x=>(object)x).ToArray());

            return true;
        }

        public override IEnumerable<double> GetAxisItemCoordinates()
        {
            if (this.PlottingSetting == null)
            {
                return null;
            }
            var range = ((INumericPlottingSettings) this.PlottingSetting).PlottingDataRange;
            var coordinates = this.PART_AxisItemsControl.Children.OfType<AxisItem>()
                .Where(x => range.IsInRange((double)x.DataContext)).Select(x => x.Coordinate);

            return coordinates;
        }

    }
}