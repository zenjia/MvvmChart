using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using MvvmCharting.Common;
using MvvmCharting.Axis;
using MvvmCharting.GridLine;
using MvvmCharting.Series;
using MvvmCharting.WpfFX.Series;

namespace MvvmCharting.WpfFX
{


    /// <summary>
    /// A Cartesian 2D Chart.
    /// This is the host for everything: SeriesChart,
    /// XAxis & YAxis, GridLineControl, Legend, CrossHair...
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_PlottingCanvas", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_HorizontalCrossHair", Type = typeof(Line))]
    [TemplatePart(Name = "PART_VerticalCrossHair", Type = typeof(Line))]
    [TemplatePart(Name = "PART_GridLineHolder", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_LegendHolder", Type = typeof(ContentControl))]
    public class Chart : Control, IXAxisOwner, IYAxisOwner
    {
        static Chart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chart), new FrameworkPropertyMetadata(typeof(Chart)));
        }

        private static readonly string sPART_Root = "PART_Root";
        private static readonly string sPART_PlottingCanvas = "PART_PlottingCanvas";

        private static readonly string sPART_HorizontalCrossHair = "PART_HorizontalCrossHair";
        private static readonly string sPART_VerticalCrossHair = "PART_VerticalCrossHair";

        private static readonly string sPART_GridLineHolder = "PART_GridLineHolder";
        private static readonly string sPART_LegendHolder = "PART_LegendHolder";


        private SeriesCollectionControl Part_SeriesCollectionControl;
        private Grid PART_Root;
        private Grid PART_PlottingCanvas;

        private Line PART_HorizontalCrossHair;
        private Line PART_VerticalCrossHair;

        private ContentControl PART_GridLineHolder;
        private ContentControl PART_LegendHolder;

        public Chart()
        {
            
        }

        #region overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.Part_SeriesCollectionControl = (SeriesCollectionControl)GetTemplateChild("Part_SeriesCollectionControl");
            if (this.Part_SeriesCollectionControl != null)
            {
                this.Part_SeriesCollectionControl.IsXAxisCategory = this.XAxis is ICategoryAxis;

                this.Part_SeriesCollectionControl.GlobalXValueRangeChanged += SeriesCollectionControlGlobalXValueRangeChanged;
                this.Part_SeriesCollectionControl.GlobalYValueRangeChanged += SeriesCollectionControlGlobalYValueRangeChanged;

                this.Part_SeriesCollectionControl.SetBinding(SeriesCollectionControl.SeriesTemplateProperty,
                    new Binding(nameof(this.SeriesTemplate)) { Source = this });
                this.Part_SeriesCollectionControl.SetBinding(SeriesCollectionControl.SeriesTemplateSelectorProperty,
                    new Binding(nameof(this.SeriesTemplateSelector)) { Source = this });
                this.Part_SeriesCollectionControl.SetBinding(SeriesCollectionControl.SeriesItemsSourceProperty,
                    new Binding(nameof(this.SeriesItemsSource)) { Source = this });

                this.Part_SeriesCollectionControl.StackMode = this.SeriesStackMode;

                OnIsChartUpdatingChanged();
            }


            this.PART_Root = (Grid)GetTemplateChild(sPART_Root);
            this.PART_PlottingCanvas = (Grid)GetTemplateChild(sPART_PlottingCanvas);

            OnXAxisPropertyChanged(null, this.XAxis);
            OnYAxisPropertyChanged(null, this.YAxis);
            OnBackgroundImageChanged(null, this.BackgroundElement);

            this.PART_HorizontalCrossHair = (Line)GetTemplateChild(sPART_HorizontalCrossHair);
            if (this.PART_HorizontalCrossHair != null)
            {
                this.PART_HorizontalCrossHair.SetBinding(StyleProperty,
                    new Binding(nameof(this.HorizontalCrossHairLineStyle)) { Source = this });
                this.PART_HorizontalCrossHair.SetBinding(Line.X2Property,
                    new Binding(nameof(this.ActualWidth)) { Source = this.PART_PlottingCanvas });

            }

            this.PART_VerticalCrossHair = (Line)GetTemplateChild(sPART_VerticalCrossHair);
            if (this.PART_VerticalCrossHair != null)
            {
                this.PART_VerticalCrossHair.SetBinding(StyleProperty,
                    new Binding(nameof(this.VerticalCrossHairLineStyle)) { Source = this });
                this.PART_VerticalCrossHair.SetBinding(Line.Y2Property,
                    new Binding(nameof(this.ActualHeight)) { Source = this.PART_PlottingCanvas });
            }

            this.PART_GridLineHolder = (ContentControl)GetTemplateChild(sPART_GridLineHolder);


            OnGridLineControlChanged();

            this.PART_PlottingCanvas.MouseMove += PlottingCanvasMouseMove;
            this.PART_PlottingCanvas.MouseLeave += PlottingCanvasMouseLeave;
            this.PART_PlottingCanvas.SizeChanged += PartPlottingCanvasSizeChanged;

            this.PART_LegendHolder = (ContentControl)GetTemplateChild(sPART_LegendHolder);
            OnLegendChanged();
        }

        private void SeriesCollectionControlGlobalYValueRangeChanged(Range obj)
        {
            UpdateActualPlottingYValueRange();
        }

        private void SeriesCollectionControlGlobalXValueRangeChanged(Range obj)
        {
            UpdateActualPlottingXValueRange();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == PaddingProperty)
            {
                TryUpdatePlottingSettings(AxisType.X);
                TryUpdatePlottingSettings(AxisType.Y);
            }
            else if (e.Property == MarginProperty)
            {
                TryUpdatePlottingSettings(AxisType.X);
                TryUpdatePlottingSettings(AxisType.Y);
            }
            else if (e.Property == BorderThicknessProperty)
            {
                TryUpdatePlottingSettings(AxisType.X);
                TryUpdatePlottingSettings(AxisType.Y);
            }
        }
        #endregion

        #region event handlers
        //private void Sr_PropertyChanged(object sender, string propertyName)
        //{
        //    var sr = (ISeries)sender;
        //    if (propertyName == nameof(sr.IsHighlighted))
        //    {
        //        this.Legend.OnItemHighlightChanged(sr.DataContext, sr.IsHighlighted);
        //    }

        //}

        private void PartPlottingCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (!this.IsLoaded)
            {
                return;
            }





            if (e.WidthChanged)
            {
                TryUpdatePlottingSettings(AxisType.X);
            }

            if (e.HeightChanged)
            {
                TryUpdatePlottingSettings(AxisType.Y);
            }
        }
        #endregion

        #region XMinimum & XMaximum & YMinimum & YMaximum
        /// <summary>
        /// The minimum independent value should be plotted.
        /// </summary>
        public double XMinimum
        {
            get { return (double)GetValue(XMinimumProperty); }
            set { SetValue(XMinimumProperty, value); }
        }
        public static readonly DependencyProperty XMinimumProperty =
            DependencyProperty.Register("XMinimum", typeof(double), typeof(Chart), new PropertyMetadata(double.NaN, OnXMinimumOrXMaximumPropertyChanged));

        /// <summary>
        /// The maximum independent value should be plotted.
        /// </summary>
        public double XMaximum
        {
            get { return (double)GetValue(XMaximumProperty); }
            set { SetValue(XMaximumProperty, value); }
        }
        public static readonly DependencyProperty XMaximumProperty =
            DependencyProperty.Register("XMaximum", typeof(double), typeof(Chart), new PropertyMetadata(double.NaN, OnXMinimumOrXMaximumPropertyChanged));

        private static void OnXMinimumOrXMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).UpdateActualPlottingXValueRange();
        }

        /// <summary>
        /// The minimum dependent value should be plotted.
        /// </summary>
        public double YMinimum
        {
            get { return (double)GetValue(YMinimumProperty); }
            set { SetValue(YMinimumProperty, value); }
        }
        public static readonly DependencyProperty YMinimumProperty =
            DependencyProperty.Register("YMinimum", typeof(double), typeof(Chart), new PropertyMetadata(double.NaN, OnYMinimumOrYMaximumPropertyChanged));

        /// <summary>
        /// The maximum dependent value should be plotted.
        /// </summary>
        public double YMaximum
        {
            get { return (double)GetValue(YMaximumProperty); }
            set { SetValue(YMaximumProperty, value); }
        }
        public static readonly DependencyProperty YMaximumProperty =
            DependencyProperty.Register("YMaximum", typeof(double), typeof(Chart), new PropertyMetadata(double.NaN));

        private static void OnYMinimumOrYMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).UpdateActualPlottingYValueRange();
        }
        #endregion

        #region SeriesDataTemplate & SeriesTemplateSelector
        public DataTemplate SeriesTemplate
        {
            get { return (DataTemplate)GetValue(SeriesTemplateProperty); }
            set { SetValue(SeriesTemplateProperty, value); }
        }

        public static readonly DependencyProperty SeriesTemplateProperty =
            SeriesCollectionControl.SeriesTemplateProperty.AddOwner(typeof(Chart));

        public DataTemplateSelector SeriesTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SeriesTemplateSelectorProperty); }
            set { SetValue(SeriesTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty SeriesTemplateSelectorProperty =
            SeriesCollectionControl.SeriesTemplateSelectorProperty.AddOwner(typeof(Chart));
        #endregion

        #region SeriesItemsSource
        /// <summary>
        /// Represents the data for a list of series(<see cref="SeriesHostHost"/>). 
        /// </summary>
        public IList SeriesItemsSource
        {
            get { return (IList)GetValue(SeriesItemsSourceProperty); }
            set { SetValue(SeriesItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty SeriesItemsSourceProperty =
            SeriesCollectionControl.SeriesItemsSourceProperty.AddOwner(typeof(Chart));
        #endregion

        #region SeriesStackMode
        public StackMode SeriesStackMode
        {
            get { return (StackMode)GetValue(SeriesStackModeProperty); }
            set { SetValue(SeriesStackModeProperty, value); }
        }
        public static readonly DependencyProperty SeriesStackModeProperty =
            DependencyProperty.Register("SeriesStackMode", typeof(StackMode), typeof(Chart), new PropertyMetadata(StackMode.None, OnSeriesStackModePropertyChanged));

        private static void OnSeriesStackModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).OnSeriesStackModeChanged();
        }

        private void OnSeriesStackModeChanged()
        {
            BatchExecute(() =>
            {


                Reset();



                if (this.Part_SeriesCollectionControl != null)
                {
                    this.Part_SeriesCollectionControl.StackMode = this.SeriesStackMode;
                }


            });
        }

        private void BatchExecute(Action action)
        {
            if (!this.IsChartUpdating)
            {
                SetCurrentValue(IsChartUpdatingProperty, true);
                try
                {
                    action.Invoke();
                }
                finally
                {
                    SetCurrentValue(IsChartUpdatingProperty, false);
                }
            }
            else
            {
                action.Invoke();
            }
        }
        #endregion

        #region Plotting Range
        /// <summary> Specify the padding to the <see cref="ActualPlottingXValueRange"/>:
        /// <list type="bullet">
        /// <item>
        ///   <see cref="P:XValuePadding.X"/> - padding to the <see cref="P:ActualPlottingXValueRange.Min"/> 
        /// </item>
        /// <item>
        ///  <see cref="P:XValuePadding.Y"/> - padding to the <see cref="P:ActualPlottingXValueRange.Max"/> 
        /// </item>
        /// </list>
        /// <see cref="PlottingXValueRange"/> is composed of <see cref="ActualPlottingXValueRange"/> plus <see cref="XValuePadding"/>
        /// </summary>
        public Point XValuePadding
        {
            get { return (Point)GetValue(XValuePaddingProperty); }
            set { SetValue(XValuePaddingProperty, value); }
        }
        public static readonly DependencyProperty XValuePaddingProperty =
            DependencyProperty.Register("XValuePadding", typeof(Point), typeof(Chart), new PropertyMetadata(new Point(0,0), OnPlottingXValuePaddingPropertyChanged));

        private static void OnPlottingXValuePaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).UpdatePlottingXValueRange();
        }

        /// <summary> Specify the padding to the <see cref="ActualPlottingYValueRange"/>:
        /// <list type="bullet">
        /// <item>
        ///   <see cref="P:YValuePadding.X"/> - padding to the <see cref="P:ActualPlottingYValueRange.Min"/> 
        /// </item>
        /// <item>
        ///  <see cref="P:YValuePadding.Y"/> - padding to the <see cref="P:ActualPlottingYValueRange.Max"/> 
        /// </item>
        /// </list>
        /// <see cref="PlottingYValueRange"/> is composed of <see cref="ActualPlottingYValueRange"/> plus <see cref="YValuePadding"/>
        /// </summary>
        public Point YValuePadding
        {
            get { return (Point)GetValue(YValuePaddingProperty); }
            set { SetValue(YValuePaddingProperty, value); }
        }
        public static readonly DependencyProperty YValuePaddingProperty =
            DependencyProperty.Register("YValuePadding", typeof(Point), typeof(Chart), new PropertyMetadata(new Point(0, 0), OnPlottingYValuePaddingPropertyChanged));

        private static void OnPlottingYValuePaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            ((Chart)d).UpdatePlottingYValueRange();
        }


        private Range _plottingXValueRange = Range.Empty;
        /// <summary>
        /// The entire independent value range(min & max) used to plot chart 
        /// including <see cref="XValuePadding"/>
        /// </summary>
        public Range PlottingXValueRange
        {
            get
            {

                return this._plottingXValueRange;
            }
            private set
            {
                if (this._plottingXValueRange != value)
                {
                    this._plottingXValueRange = value;


                    this.Part_SeriesCollectionControl.SetPlottingValueRange(Orientation.Horizontal,
                        this.PlottingXValueRange);

                    TryUpdatePlottingSettings(AxisType.X);
                }
            }
        }

        private Range _plottingYValueRange = Range.Empty;
        /// <summary>
        /// The entire dependent value range(min & max) used to plot chart 
        /// including <see cref="XValuePadding"/>
        /// </summary>
        public Range PlottingYValueRange
        {
            get { return this._plottingYValueRange; }
            private set
            {
                if (this._plottingYValueRange != value)
                {
                    this._plottingYValueRange = value;

                    TryUpdatePlottingSettings(AxisType.Y);
                    this.Part_SeriesCollectionControl.SetPlottingValueRange(Orientation.Vertical,
                         this.PlottingYValueRange);


                }
            }
        }

        private Range _actualPlottingXValueRange = Range.Empty;
        public Range ActualPlottingXValueRange
        {
            get { return this._actualPlottingXValueRange; }
            private set
            {
                if (this._actualPlottingXValueRange != value)
                {
                    this._actualPlottingXValueRange = value;
                    UpdatePlottingXValueRange();
                }

            }
        }

        private Range _actualPlottingYValueRange = Range.Empty;
        public Range ActualPlottingYValueRange
        {
            get { return this._actualPlottingYValueRange; }
            private set
            {
                if (this._actualPlottingYValueRange != value)
                {
                    this._actualPlottingYValueRange = value;
                    UpdatePlottingYValueRange();
                }

            }
        }

        private void UpdatePlottingXValueRange()
        {
            var range = new Range(this.ActualPlottingXValueRange.Min - this.XValuePadding.X,
                this.ActualPlottingXValueRange.Max + this.XValuePadding.Y);

            if (range.Span <= 0)
            {
                throw new MvvmChartException($"Invalid XValueRange: {range}");
            }
            this.PlottingXValueRange = range;
        }

        private void UpdatePlottingYValueRange()
        {
            var range = new Range(this.ActualPlottingYValueRange.Min - this.XValuePadding.X,
                this.ActualPlottingYValueRange.Max + this.YValuePadding.Y);

            if (range.Span <= 0)
            {
                throw new MvvmChartException($"Invalid YValueRange: {range}");
            }
            this.PlottingYValueRange = range;
        }

        private void UpdateActualPlottingXValueRange()
        {
            if (this.Part_SeriesCollectionControl == null)
            {
                this.PlottingXValueRange = Range.Empty;
                return;
            }

            double min = !this.XMinimum.IsNaN() ? this.XMinimum : this.Part_SeriesCollectionControl.GlobalXValueRange.Min;
            double max = !this.XMaximum.IsNaN() ? this.XMaximum : this.Part_SeriesCollectionControl.GlobalXValueRange.Max;

            if (min.IsNaN() && max.IsNaN())
            {
                this.ActualPlottingXValueRange = Range.Empty;
                return;
            }



            this.ActualPlottingXValueRange = new Range(min, max);

        }

        private void UpdateActualPlottingYValueRange()
        {
            if (this.Part_SeriesCollectionControl == null)
            {
                this.PlottingXValueRange = Range.Empty;
                return;
            }

            double min = !this.YMinimum.IsNaN() ? this.YMinimum : this.Part_SeriesCollectionControl.GlobalYValueRange.Min;
            double max = !this.YMaximum.IsNaN() ? this.YMaximum : this.Part_SeriesCollectionControl.GlobalYValueRange.Max;

            if (min.IsNaN() && max.IsNaN())
            {
                this.ActualPlottingYValueRange = Range.Empty;
                return;
            }

            this.ActualPlottingYValueRange = GetProperPlottingYRange(new Range(min, max));

        }

        private Range GetProperPlottingYRange(Range newRange)
        {

            switch (this.SeriesStackMode)
            {
                case StackMode.None:
                    return newRange;

                case StackMode.Stacked:
                    return new Range(0.00, newRange.Max);

                case StackMode.Stacked100:
                    return new Range(0.00, 1.00);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        #endregion

        #region PlottingSettings
        public event Action<PlottingRangeBase> HorizontalSettingChanged;
        public event Action<PlottingRangeBase> VerticalSettingChanged;

        private PlottingRangeBase _horizontalPlottingRangeSetting;
        private PlottingRangeBase _verticalPlottingRangeSetting;

        private PlottingRangeBase GetPlottingSettings(AxisType orientation)
        {
            if (this.PART_PlottingCanvas == null)
            {

                return null;
            }

            Range plotingDataRange, valuePadding;
            IList<object> plottingItemValues = null;
            bool isCategory;
            switch (orientation)
            {
                case AxisType.X:
                    if (this.XAxis == null)
                    {
                        return null;
                    }
                    valuePadding = new Range(this.XValuePadding.X, this.XValuePadding.Y);
 
                    isCategory = this.XAxis is ICategoryAxis;
                    plotingDataRange = this.ActualPlottingXValueRange;

                    var sr = this.Part_SeriesCollectionControl.GetSeries().FirstOrDefault();

                    plottingItemValues = sr?.ItemsSource.OfType<object>().Select(x => sr.GetXRawValueForItem(x))
                        .ToArray();
                    break;


                case AxisType.Y:
                    if (this.YAxis == null)
                    {
                        return null;
                    }
                    valuePadding = new Range(this.YValuePadding.X, this.YValuePadding.Y);
                    isCategory = this.YAxis is ICategoryAxis;
                    plotingDataRange = this.ActualPlottingYValueRange;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

            if (!plotingDataRange.IsInvalid)
            {
                if (isCategory)
                {

                    if (plottingItemValues == null ||
                        plottingItemValues.Count == 0 ||
                        plottingItemValues.Count != plottingItemValues.Distinct().Count())
                    {
                        return null;
                    }

                    return new CategoryPlottingRange(plottingItemValues, valuePadding);

                }
                else
                {
                    return new NumericPlottingRange(plotingDataRange, valuePadding);

                }
            }

            return null;
        }

        /// <summary>
        /// Detect if plotting setting changed. If true, then re-plot the axis & gridline
        /// </summary>
        /// <param name="orientation"></param>
        private void TryUpdatePlottingSettings(AxisType orientation)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            var newValue = GetPlottingSettings(orientation);
            switch (orientation)
            {
                case AxisType.X:
                    if (this._horizontalPlottingRangeSetting != newValue)
                    {
                        this._horizontalPlottingRangeSetting = newValue;
                        this.HorizontalSettingChanged?.Invoke(newValue);
                    }
                    break;
                case AxisType.Y:
                    if (this._verticalPlottingRangeSetting != newValue)
                    {
                        this._verticalPlottingRangeSetting = newValue;
                        this.VerticalSettingChanged?.Invoke(newValue);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

        }
        #endregion

        #region GridLine
        public IGridLineControl GridLineControl
        {
            get { return (IGridLineControl)GetValue(GridLineControlProperty); }
            set { SetValue(GridLineControlProperty, value); }
        }
        public static readonly DependencyProperty GridLineControlProperty =
            DependencyProperty.Register("GridLineControl", typeof(IGridLineControl), typeof(Chart), new PropertyMetadata(null, OnGridLineControlPropertyChanged));

        private static void OnGridLineControlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).OnGridLineControlChanged();
        }

        private void OnGridLineControlChanged()
        {

            if (this.PART_GridLineHolder != null)
            {

                this.PART_GridLineHolder.Content = this.GridLineControl;
                this.GridLineControl?.OnAxisItemCoordinateChanged(AxisType.Y, this.YAxis?.GetAxisItemCoordinates());
                this.GridLineControl?.OnAxisItemCoordinateChanged(AxisType.X, this.XAxis?.GetAxisItemCoordinates());
            }

        }

        public Visibility GridLineControlVisibility
        {
            get { return (Visibility)GetValue(GridLineControlVisibilityProperty); }
            set { SetValue(GridLineControlVisibilityProperty, value); }
        }
        public static readonly DependencyProperty GridLineControlVisibilityProperty =
            DependencyProperty.Register("GridLineControlVisibility", typeof(Visibility), typeof(Chart), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Axises
        public IAxisNS XAxis
        {
            get { return (IAxisNS)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(IAxisNS), typeof(Chart), new PropertyMetadata(null, OnXAxisPropertyChanged));

        private static void OnXAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).OnXAxisPropertyChanged((IAxisNS)e.OldValue, (IAxisNS)e.NewValue);
        }
        private void OnXAxisPropertyChanged(IAxisNS oldValue, IAxisNS newValue)
        {
            if (this.PART_Root == null)
            {
                return;
            }

            if (oldValue != null)
            {

                this.PART_Root.Children.Remove(oldValue as UIElement);
                oldValue.AxisPlacementChanged -= OnAxisPlacementChanged;
            }

            if (newValue != null)
            {
                if (this.Part_SeriesCollectionControl != null)
                {
                    this.Part_SeriesCollectionControl.IsXAxisCategory = newValue is ICategoryAxis;
                }


                if (this.PART_Root.Children.Contains(newValue as UIElement))
                {
                    return;
                }
                if (newValue.Placement == AxisPlacement.None)
                {
                    newValue.Placement = AxisPlacement.Bottom;
                }

                this.PART_Root.Children.Add(newValue as UIElement);
                newValue.Owner = this;
                newValue.Orientation = AxisType.X;
                newValue.AxisPlacementChanged += OnAxisPlacementChanged;
                OnAxisPlacementChanged(newValue);
            }
        }

        public IAxisNS YAxis
        {
            get { return (IAxisNS)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }
        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(IAxisNS), typeof(Chart), new PropertyMetadata(null, OnYAxisPropertyChanged));
        private static void OnYAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).OnYAxisPropertyChanged((IAxisNS)e.OldValue, (IAxisNS)e.NewValue);
        }
        private void OnYAxisPropertyChanged(IAxisNS oldValue, IAxisNS newValue)
        {
            if (newValue is ICategoryAxis)
            {
                throw new MvvmChartException("CategoryAxis cannot be used as YAxis!");
            }

            if (this.PART_Root == null)
            {
                return;
            }

            if (oldValue != null)
            {
                this.PART_Root.Children.Remove(oldValue as UIElement);
                oldValue.AxisPlacementChanged -= OnAxisPlacementChanged;
            }

            if (newValue != null)
            {
                if (this.PART_Root.Children.Contains(newValue as UIElement))
                {
                    return;
                }

                if (newValue.Placement == AxisPlacement.None)
                {
                    newValue.Placement = AxisPlacement.Left;
                }


                this.PART_Root.Children.Add(newValue as UIElement);
                newValue.Owner = this;
                newValue.Orientation = AxisType.Y;
                newValue.AxisPlacementChanged += OnAxisPlacementChanged;
                OnAxisPlacementChanged(newValue);
            }
        }

        private void OnAxisPlacementChanged(IAxisNS obj)
        {

            var axis = obj as UIElement;
            switch (obj.Orientation)
            {
                case AxisType.X:
                    Grid.SetColumn(axis, 1);
                    switch (obj.Placement)
                    {
                        case AxisPlacement.Bottom:
                            Grid.SetRow(axis, 2);
                            break;
                        case AxisPlacement.Top:
                            Grid.SetRow(axis, 0);
                            break;
                        default:
                            throw new NotSupportedException($"XAxis does not support '{obj.Placement}' placement!");
                    }
                    break;
                case AxisType.Y:
                    Grid.SetRow(axis, 1);

                    switch (obj.Placement)
                    {
                        case AxisPlacement.Left:
                            Grid.SetColumn(axis, 0);
                            break;
                        case AxisPlacement.Right:
                            Grid.SetColumn(axis, 2);
                            break;
                        default:
                            throw new NotSupportedException($"YAxis does not support '{obj.Placement}' placement!");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        #endregion

        #region BackgroundImage
        /// <summary>
        /// Represents pluggable background UIElement.
        /// User can plug any UIElement(include image) here.
        /// </summary>
        public UIElement BackgroundElement
        {
            get { return (UIElement)GetValue(BackgroundElementProperty); }
            set { SetValue(BackgroundElementProperty, value); }
        }
        public static readonly DependencyProperty BackgroundElementProperty =
            DependencyProperty.Register("BackgroundElement", typeof(UIElement), typeof(Chart), new PropertyMetadata(null, OnBackgroundImagePropertyChanged));

        private static void OnBackgroundImagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ((Chart)d).OnBackgroundImageChanged((UIElement)e.OldValue, (UIElement)e.NewValue);
        }

        private void OnBackgroundImageChanged(UIElement oldValue, UIElement newValue)
        {
            if (this.PART_PlottingCanvas == null)
            {
                return;
            }

            if (this.PART_PlottingCanvas.Children.Contains(oldValue))
            {
                this.PART_PlottingCanvas.Children.Remove(oldValue);
            }

            if (newValue != null && !this.PART_PlottingCanvas.Children.Contains(newValue))
            {
                this.PART_PlottingCanvas.Children.Insert(0, newValue);
            }

        }
        #endregion

        #region cross hair
        public Visibility HorizontalCrossHairVisibility
        {
            get { return (Visibility)GetValue(HorizontalCrossHairVisibilityProperty); }
            set { SetValue(HorizontalCrossHairVisibilityProperty, value); }
        }
        public static readonly DependencyProperty HorizontalCrossHairVisibilityProperty =
            DependencyProperty.Register("HorizontalCrossHairVisibility", typeof(Visibility), typeof(Chart), new PropertyMetadata(Visibility.Visible));

        public Visibility VerticalCrossHairVisiblity
        {
            get { return (Visibility)GetValue(VerticalCrossHairVisiblityProperty); }
            set { SetValue(VerticalCrossHairVisiblityProperty, value); }
        }
        public static readonly DependencyProperty VerticalCrossHairVisiblityProperty =
            DependencyProperty.Register("VerticalCrossHairVisiblity", typeof(Visibility), typeof(Chart), new PropertyMetadata(Visibility.Visible));

        public Style HorizontalCrossHairLineStyle
        {
            get { return (Style)GetValue(HorizontalCrossHairLineStyleProperty); }
            set { SetValue(HorizontalCrossHairLineStyleProperty, value); }
        }
        public static readonly DependencyProperty HorizontalCrossHairLineStyleProperty =
            DependencyProperty.Register("HorizontalCrossHairLineStyle", typeof(Style), typeof(Chart), new PropertyMetadata(null));

        public Style VerticalCrossHairLineStyle
        {
            get { return (Style)GetValue(VerticalCrossHairLineStyleProperty); }
            set { SetValue(VerticalCrossHairLineStyleProperty, value); }
        }
        public static readonly DependencyProperty VerticalCrossHairLineStyleProperty =
            DependencyProperty.Register("VerticalCrossHairLineStyle", typeof(Style), typeof(Chart), new PropertyMetadata(null));

        private void PlottingCanvasMouseMove(object sender, MouseEventArgs e)
        {
            bool isHorizontalCrossHairVisible = this.HorizontalCrossHairVisibility == Visibility.Visible;
            bool isVerticalCrossHairVisible = this.VerticalCrossHairVisiblity == Visibility.Visible;

            if (!isHorizontalCrossHairVisible &&
                !isVerticalCrossHairVisible)
            {
                return;
            }

            var mousePoint = e.GetPosition(this.PART_PlottingCanvas);

            if (isHorizontalCrossHairVisible)
            {
                MoveCrossHairLine(Orientation.Horizontal, mousePoint.Y);
            }

            if (isVerticalCrossHairVisible)
            {
                MoveCrossHairLine(Orientation.Vertical, mousePoint.X);
            }


        }

        private void PlottingCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            if (this.PART_HorizontalCrossHair.Visibility != Visibility.Collapsed)
            {
                this.PART_HorizontalCrossHair.Visibility = Visibility.Collapsed;
            }

            if (this.PART_VerticalCrossHair.Visibility != Visibility.Collapsed)
            {
                this.PART_VerticalCrossHair.Visibility = Visibility.Collapsed;
            }
        }

        public void MoveCrossHairLine(Orientation orientation, double offset)
        {
            Line crossHairLine = orientation == Orientation.Horizontal
                ? this.PART_HorizontalCrossHair
                : this.PART_VerticalCrossHair;

            if (crossHairLine == null)
            {
                return;
            }

            if (offset.IsNaN())
            {
                if (crossHairLine.Visibility != Visibility.Collapsed)
                {
                    crossHairLine.Visibility = Visibility.Collapsed;
                }

                return;
            }

            if (crossHairLine == this.PART_HorizontalCrossHair)
            {
                crossHairLine.Y1 = offset;
                crossHairLine.Y2 = offset;
            }
            else
            {
                crossHairLine.X1 = offset;
                crossHairLine.X2 = offset;
            }

            if (crossHairLine.Visibility != Visibility.Visible)
            {
                crossHairLine.Visibility = Visibility.Visible;
            }


        }
        #endregion

        #region Legend
        public LegendControl Legend
        {
            get { return (LegendControl)GetValue(LegendProperty); }
            set { SetValue(LegendProperty, value); }
        }
        public static readonly DependencyProperty LegendProperty =
            DependencyProperty.Register("Legend", typeof(LegendControl), typeof(Chart), new PropertyMetadata(null, OnLegendPropertyChanged));

        private static void OnLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).OnLegendChanged();
        }

        private void OnLegendChanged()
        {
            if (this.Legend != null)
            {
                this.Legend.SetBinding(LegendControl.LegendItemTemplateProperty, new Binding(nameof(this.LegendItemTemplate)) { Source = this });
                this.Legend.SetBinding(LegendControl.ItemsSourceProperty, new Binding(nameof(this.SeriesItemsSource)) { Source = this });
            }

            if (this.PART_LegendHolder != null)
            {
                this.PART_LegendHolder.Content = this.Legend;
            }
        }

        //private void Legend_LegendItemHighlighChanged(LegendItemControl sender, bool newValue)
        //{
        //    var item = sender.DataContext;
        //    var sr = this.PART_SeriesItemsControl?.TryGetElementForItem(item) as ISeries;

        //    if (sr != null)
        //    {
        //        sr.IsHighlighted = newValue;
        //    }
        //}

        public DataTemplate LegendItemTemplate
        {
            get { return (DataTemplate)GetValue(LegendItemTemplateProperty); }
            set { SetValue(LegendItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty LegendItemTemplateProperty =
            DependencyProperty.Register("LegendItemTemplate", typeof(DataTemplate), typeof(Chart), new PropertyMetadata(null));






        public Visibility LegendVisibility
        {
            get { return (Visibility)GetValue(LegendVisibilityProperty); }
            set { SetValue(LegendVisibilityProperty, value); }
        }
        public static readonly DependencyProperty LegendVisibilityProperty =
            DependencyProperty.Register("LegendVisibility", typeof(Visibility), typeof(Chart), new PropertyMetadata(Visibility.Visible));



        #endregion

        #region IsChartUpdating
        /// <summary>
        /// When collection or chart setting is changing, set this to true can
        /// reduce some performance-hitting, duplicated operation. 
        /// </summary>
        public bool IsChartUpdating
        {
            get { return (bool)GetValue(IsChartUpdatingProperty); }
            set { SetValue(IsChartUpdatingProperty, value); }
        }
        public static readonly DependencyProperty IsChartUpdatingProperty =
            DependencyProperty.Register("IsChartUpdating", typeof(bool), typeof(Chart), new PropertyMetadata(false, OnIsChartUpdatingPropertyChanged));

        private static void OnIsChartUpdatingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).OnIsChartUpdatingChanged();
        }

        private void OnIsChartUpdatingChanged()
        {
            if (this.Part_SeriesCollectionControl != null)
            {
                this.Part_SeriesCollectionControl.IsSeriesCollectionChanging = this.IsChartUpdating;
                this.Part_SeriesCollectionControl.Refresh();
            }
        }
        #endregion

        /// <summary>
        /// Called when x-axis or y-axis has updated the coordinates of its items.
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="ticks"></param>
        public void OnAxisItemsCoordinateChanged(AxisType orientation, IEnumerable<double> ticks)
        {
            this.GridLineControl?.OnAxisItemCoordinateChanged(orientation, ticks);
        }

        private void Reset()
        {

            this.Part_SeriesCollectionControl.Reset();

            switch (this.SeriesStackMode)
            {
                case StackMode.None:
                    this.PlottingYValueRange = Range.Empty;
                    break;
                case StackMode.Stacked:
                    this.PlottingYValueRange = new Range(0, double.NaN);
                    break;
                case StackMode.Stacked100:
                    this.PlottingYValueRange = new Range(0, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


}
