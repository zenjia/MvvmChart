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
            TryLoadAxisItemDrawingParams();
        }
         
        protected override void UpdateAxisDrawingSettings()
        {
            if (/*!this.IsLoaded ||*/
                this.PlottingSetting == null)
            {
                return;
            }

            var range = ((ILinearPlottingSettings) this.PlottingSetting).PlottingDataRange;
            var length = this.PlottingSetting.GetAvailablePlottingSize();
            if (range.IsInvalid || length.IsNaNOrZero())
            {
                throw new NotImplementedException();
            }

            var axisDrawingSettings = new AxisDrawingSettings(this.TickCount, this.TickInterval, range, length);
            this.DrawingSettings = axisDrawingSettings;
        }

 
        protected override void DoUpdateAxisItemsCoordinate()
        {
            var span = ((ILinearAxisDrawingSettings) this.DrawingSettings).PlottingDataRange.Span;
            var length = this.DrawingSettings.PlottingLength;
            var uLen = length / span;


            foreach (var item in this.ItemDrawingParams)
            {
                var coordinate = ((double)item.Value - ((ILinearAxisDrawingSettings) this.DrawingSettings).PlottingDataRange.Min) * uLen;
                if (this.Orientation == AxisType.Y)
                {
                    coordinate = length - coordinate;
                }

                item.Coordinate = coordinate;


            }
        }


        private IList<double> GetItemDataValues(double startValue, double tickInterval, int tickCount)
        {
            var chartRange = ((ILinearPlottingSettings) this.PlottingSetting).PlottingDataRange;

            var arr = Enumerable.Range(0, tickCount)
                .Select(i => startValue + i * tickInterval)
                .Where(x => chartRange.IsInRange(x))
                .ToArray();

            return arr;
        }


        protected override bool DoLoadAxisItemDrawingParams()
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

                dataValues = GetItemDataValues(((ILinearAxisDrawingSettings)drawingSettings).PlottingDataRange.Min, drawingSettings.ActualTickInterval, drawingSettings.ActualTickCount);


            }

            UpdateItemDrawingParams(dataValues.Select(x=>(object)x).ToArray());

            return true;
        }


      

        public override IEnumerable<double> GetAxisItemCoordinates()
        {
            if (this.PlottingSetting == null)
            {
                return null;
            }
            var range = ((ILinearPlottingSettings) this.PlottingSetting).PlottingDataRange;
            var coordinates = this.ItemDrawingParams.Where(x => range.IsInRange((double)x.Value)).Select(x => x.Coordinate);

            return coordinates;
        }

    }
}