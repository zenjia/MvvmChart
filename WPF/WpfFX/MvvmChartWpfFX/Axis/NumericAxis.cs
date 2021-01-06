using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MvvmCharting.Axis;

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
            if (this.PlottingRangeSetting == null)
            {
                return;
            }

            var numericPlottingSettings = (NumericPlottingRange)this.PlottingRangeSetting;


            var length = this.Orientation == AxisType.X ? this.ActualWidth : this.ActualHeight;



            var axisDrawingSettings = new NumericAxisDrawingSettings(this.TickCount, this.TickInterval,
                numericPlottingSettings, length);
            this.DrawingSettings = axisDrawingSettings;
        }


        protected override void DoUpdateAxisItemsCoordinate()
        {
   

            foreach (IAxisItem item in GetAllAxisItems())
            {
                var coordinate = this.DrawingSettings.CalculateCoordinate((double)item.DataContext, this.Orientation);

                item.Coordinate = coordinate;
 
            }
        }



        protected override bool LoadAxisItems()
        {

            IList<object> values;
            if (this.ExplicitTicks != null)
            {
                values = this.ExplicitTicks.Select(x=>(object)x).ToArray();
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

                values = drawingSettings.GetPlottingValues().ToArray();
            }

            DoUpdateAxisItems(values);

            return true;
        }

        public override IEnumerable<double> GetAxisItemCoordinates()
        {
            if (this.PlottingRangeSetting == null)
            {
                return null;
            }

            return this.PART_AxisItemsControl.Children.OfType<AxisItem>()
                .Select(x => x.Coordinate);

 
        }

    }
}