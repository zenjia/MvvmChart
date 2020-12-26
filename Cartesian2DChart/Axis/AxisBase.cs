using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Cartesian2DChart.Axis;
using MvvmChart.Common;
using MvvmChart.Common.Axis;

namespace MvvmCharting.Axis
{
    /// <summary>
    /// The base class for XAxis and YAxis.
    /// It is basically a numeric, linear axis, and can't handle category data type.
    /// It can handle DateTime data type by converting it to double type and back using System.Convert.
    /// Also, user can provide their own customized converters to customize Text of the labels of axis.
    /// </summary>
    [TemplatePart(Name = "PART_AxisItemsControl", Type = typeof(SlimItemsControl))]
    public class AxisBase : Control, IAxis
    {
        static AxisBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AxisBase), new FrameworkPropertyMetadata(typeof(AxisBase)));
        }

        private static readonly string sPART_AxisItemsControl = "PART_AxisItemsControl";

        private SlimItemsControl PART_AxisItemsControl;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.PART_AxisItemsControl != null)
            {
                this.PART_AxisItemsControl.ElementGenerated -= AxisItemsControlItemTemplateApplied;
            }

            this.PART_AxisItemsControl = (SlimItemsControl)GetTemplateChild(sPART_AxisItemsControl);
            if (this.PART_AxisItemsControl != null)
            {

                this.PART_AxisItemsControl.ElementGenerated += AxisItemsControlItemTemplateApplied;

                this.PART_AxisItemsControl.SetBinding(SlimItemsControl.ItemTemplateProperty,
                    new Binding(nameof(this.ItemTemplate)) {Source = this});
                this.PART_AxisItemsControl.SetBinding(SlimItemsControl.ItemTemplateSelectorProperty,
                    new Binding(nameof(this.ItemTemplateSelector)) { Source = this });

                this.PART_AxisItemsControl.ItemsSource = this.ItemDrawingParams;
                TryLoadAxisItemDrawingParams();
            }


        }



        public AxisBase()
        {
            this.ItemDrawingParams = new ObservableCollection<AxisItemDrawingParam>();



        }




        public AxisPlacement Placement
        {
            get { return (AxisPlacement)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(AxisPlacement), typeof(AxisBase), new PropertyMetadata(AxisPlacement.None, OnPlacementPropertyChange));

        private static void OnPlacementPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisBase)d).OnPlacementChange();
        }

        private IEnumerable<IAxisItem> GetAllAxisItems()
        {
            if (this.PART_AxisItemsControl==null)
            {
                return Enumerable.Empty<IAxisItem>();
            }

            return this.PART_AxisItemsControl.GetAllElements().OfType<IAxisItem>();
        }

        private void OnPlacementChange()
        {
            foreach (var axisItem in GetAllAxisItems())
            {
                axisItem.SetAxisPlacement(this.Placement);
            }

            this.AxisPlacementChanged?.Invoke(this);
        }



        public double TickInterval
        {
            get { return (double)GetValue(TickIntervalProperty); }
            set { SetValue(TickIntervalProperty, value); }
        }
        public static readonly DependencyProperty TickIntervalProperty =
            DependencyProperty.Register("TickInterval", typeof(double), typeof(AxisBase), new PropertyMetadata(double.NaN, OnTickIntervalPropertyChanged));

        private static void OnTickIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisBase)d).UpdateAxisDrawingSettings(); ;
        }


        public int TickCount
        {
            get { return (int)GetValue(TickCountProperty); }
            set { SetValue(TickCountProperty, value); }
        }
        public static readonly DependencyProperty TickCountProperty =
            DependencyProperty.Register("TickCount", typeof(int), typeof(AxisBase), new PropertyMetadata(5, OnTickCountPropertyChanged));

        private static void OnTickCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisBase)d).UpdateAxisDrawingSettings();
        }




        public IList<double> ExplicitTicks
        {
            get { return (IList<double>)GetValue(ExplicitTicksProperty); }
            set { SetValue(ExplicitTicksProperty, value); }
        }
        public static readonly DependencyProperty ExplicitTicksProperty =
            DependencyProperty.Register("ExplicitTicks", typeof(IList<double>), typeof(AxisBase), new PropertyMetadata(null, OnExplicitTicksPropertyChanged));

        private static void OnExplicitTicksPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisBase)d).OnExplicitTicksChanged((IList<double>)e.OldValue, (IList<double>)e.NewValue);
        }

        private void OnExplicitTicksChanged(IList<double> oldValue, IList<double> newValue)
        {
            TryLoadAxisItemDrawingParams();
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner(typeof(AxisBase));



        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            ItemsControl.ItemTemplateSelectorProperty.AddOwner(typeof(AxisBase));


        /// <summary>
        /// A double to string convert.
        /// The Axis only receive double values, so its the user's responsibility to provide a proper
        /// converter in order to correctly display the Label text. 
        /// If the double is converted from DateTime or DateTimeOffset, then it should be
        /// convert back to DateTime or DateTimeOffset first before it can be convert to a user-formatted string
        /// </summary>
        public IValueConverterNS LabelTextConverter
        {
            get { return (IValueConverterNS)GetValue(LabelTextConverterProperty); }
            set { SetValue(LabelTextConverterProperty, value); }
        }
        public static readonly DependencyProperty LabelTextConverterProperty =
            DependencyProperty.Register("LabelTextConverter", typeof(IValueConverterNS), typeof(AxisBase), new PropertyMetadata(null, OnLabelTextConverterPropertyChanged));

        private static void OnLabelTextConverterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
             ((AxisBase)d).OnLabelTextConverterChanged((IValueConverterNS)e.OldValue, (IValueConverterNS)e.NewValue);
        }

        private void OnLabelTextConverterChanged(IValueConverterNS oldVlue, IValueConverterNS newValue)
        {
            foreach (var axisItem in GetAllAxisItems())
            {
                axisItem.SetLabelTextConverter(this.LabelTextConverter);
            }
        }

        private IAxisOwner _owner;
        public IAxisOwner Owner
        {
            get { return this._owner; }
            set
            {
                
                if (this._owner != value)
                {
                    this._owner = value;
                    AttachHandler();
                }

            }
        }

        private AxisType _orientation;
        public AxisType Orientation
        {
            get { return this._orientation; }
            set
            {
                if (this._orientation != value)
                {
                    this._orientation = value;
                    AttachHandler();
                }

            }
        }

        private void AttachHandler()
        {
            if (this.Owner == null ||
                this.Orientation == AxisType.None)
            {
                return;
            }

            switch (this.Orientation)
            {
                case AxisType.XAxis:
                    ((IXAxisOwner)this.Owner).CanvasHorizontalSettingChanged += AxisBase_CanvasSettingChanged;
                    break;
                case AxisType.YAxis:
                    ((IYAxisOwner)this.Owner).CanvasVerticalSettingChanged += AxisBase_CanvasSettingChanged;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateAxisDrawingSettings();
        }

        private void AxisBase_CanvasSettingChanged(PlottingSettings obj)
        {
            switch (obj.Orientation)
            {
                case AxisType.XAxis:
                    this.RenderSize = new Size(obj.RenderSize, this.RenderSize.Height);
                    this.Margin = new Thickness(obj.Margin.X, this.Margin.Top, obj.Margin.Y, this.Margin.Bottom);
                    this.Padding = new Thickness(obj.Padding.X, this.Padding.Top, obj.Padding.Y, this.Padding.Bottom);
                    this.BorderThickness = new Thickness(obj.BorderThickness.X, this.BorderThickness.Top, obj.BorderThickness.Y, this.BorderThickness.Bottom);

                    break;
                case AxisType.YAxis:
                    this.RenderSize = new Size(this.RenderSize.Width, obj.RenderSize);
                    this.Margin = new Thickness(this.Margin.Left, obj.Margin.X, this.Margin.Right, obj.Margin.Y);
                    this.Padding = new Thickness(this.Padding.Left, obj.Padding.X, this.Padding.Right, obj.Padding.Y);
                    this.BorderThickness = new Thickness(this.BorderThickness.Left, obj.BorderThickness.X, this.BorderThickness.Right, obj.BorderThickness.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.PlottingSetting = obj;


        }

        private PlottingSettings _plottingSetting;
        protected PlottingSettings PlottingSetting
        {
            get { return this._plottingSetting; }

            set
            {
                if (this._plottingSetting != value)
                {
                    this._plottingSetting = value;
                    UpdateAxisDrawingSettings();
                }
            }
        }


        private AxisDrawingSettings _drawingSettings;
        internal AxisDrawingSettings DrawingSettings
        {
            get { return this._drawingSettings; }
            set
            {

                if (this._drawingSettings != value)
                {
                    this._drawingSettings = value;

                    TryLoadAxisItemDrawingParams();
                }


            }
        }


        public ObservableCollection<AxisItemDrawingParam> ItemDrawingParams { get; }



        private void UpdateAxisDrawingSettings()
        {
            if (/*!this.IsLoaded ||*/
                this.PlottingSetting == null)
            {
                return;
            }


            var range = this.PlottingSetting.PlotingDataRange;
            var length = this.PlottingSetting.GetAvailablePlottingSize();
            if (range.IsInvalid || length.IsNaNOrZero())
            {
                throw new NotImplementedException();
            }

            var axisDrawingSettings = new AxisDrawingSettings(this.TickCount, this.TickInterval, range, length);
            this.DrawingSettings = axisDrawingSettings;
        }









        private IList<double> GetItemDataValues(double startValue, double tickInterval, int tickCount)
        {
            var chartRange = this.PlottingSetting.PlotingDataRange;

            var arr = Enumerable.Range(0, tickCount)
                .Select(i => startValue + i * tickInterval)
                .Where(x => chartRange.IsInRange(x))
                .ToArray();

            return arr;
        }

        private void UpdateItemDrawingParams(IList<double> source)
        {

            var oldCt = this.ItemDrawingParams.Count;
            var newCt = source.Count;
            if (oldCt > newCt)
            {
                this.ItemDrawingParams.RemoveRange(newCt, oldCt - newCt);
            }

            for (int i = 0; i < source.Count; i++)
            {
                var newValue = source[i];
                if (i < oldCt)
                {
                    AxisItemDrawingParam item = this.ItemDrawingParams[i];
                    item.Value = newValue;
                }
                else
                {
                    AxisItemDrawingParam item = new AxisItemDrawingParam();
                    item.Value = newValue;
                    this.ItemDrawingParams.Add(item);
                }
            }




        }

        public IEnumerable<double> GetAxisItemCoordinates()
        {
            if (this.PlottingSetting == null)
            {
                return null;
            }
            var range = this.PlottingSetting.PlotingDataRange;
            var coordinates = this.ItemDrawingParams.Where(x => range.IsInRange(x.Value)).Select(x => x.Coordinate);

            return coordinates;
        }

        private void OnAxisItemCoordinateChanged()
        {

            var coordinates = GetAxisItemCoordinates();


            this.Owner.OnAxisItemsCoordinateChanged(this.Orientation, coordinates);


           
 

        }

        private AxisDrawingSettings _currentDrawingSettings;

        public bool TryLoadAxisItemDrawingParams()
        {
            if (this.PART_AxisItemsControl == null)
            {
                return false;
            }


            IList<double> dataValues;
            if (this.ExplicitTicks != null)
            {
                dataValues = this.ExplicitTicks;
                this._currentDrawingSettings = null;
            }
            else
            {
                AxisDrawingSettings drawingSettings = this.DrawingSettings;
                if (drawingSettings == null || !drawingSettings.CanUpdateAxisItems())
                {
                    return false;
                }

                if (drawingSettings.Equals(this._currentDrawingSettings))
                {
                    return false;
                }

                this._currentDrawingSettings = drawingSettings;

                dataValues = GetItemDataValues(drawingSettings.PlotingDataRange.Min, drawingSettings.ActualTickInterval, drawingSettings.ActualTickCount);


            }


            UpdateItemDrawingParams(dataValues);

         
            UpdateAxisItemsCoordinate();


            return true;
        }


        private void UpdateAxisItemsCoordinate()
        {

            if (!this.DrawingSettings.CanUpdateAxisItemsCoordinate())
            {
                throw new NotImplementedException();
            }


            var span = this.DrawingSettings.PlotingDataRange.Span;
            var length = this.DrawingSettings.PlotingLength;
            var uLen = length / span;

            var len = (this.Orientation == AxisType.YAxis) ? this.ActualHeight: this
                .ActualWidth;

  
            foreach (var item in this.ItemDrawingParams)
            {
                var coordinate = (item.Value - this.DrawingSettings.PlotingDataRange.Min) * uLen;
                if (this.Orientation == AxisType.YAxis)
                {
                    coordinate = length - coordinate;
                }

                item.Coordinate = coordinate;

 
            }


            OnAxisItemCoordinateChanged();
        }
 

        private void AxisItemsControlItemTemplateApplied(object sender, DependencyObject root)
        {
            if (!(root is IAxisItem axisItem))
            {
                throw new MvvmChartUnexpectedTypeException($"The root item of ItemTemplate of an axis must be based on '{typeof(AxisItem)}'!");
            }

            axisItem.SetLabelTextConverter(this.LabelTextConverter);
            axisItem.SetAxisPlacement(this.Placement);
 
        }





        public event Action<IAxis> AxisPlacementChanged;



    }

}
