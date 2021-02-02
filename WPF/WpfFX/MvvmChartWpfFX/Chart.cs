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
#if NETCOREAPP
using Range = MvvmCharting.Common.Range;
#endif

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
    public class Chart : Control, IChart, IXAxisOwner, IYAxisOwner
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

                this.Part_SeriesCollectionControl.Owner = this;

                this.Part_SeriesCollectionControl.ActualXPlottingRangeChanged += SeriesCollectionControl_ActualXPlottingRangeChanged;
                this.Part_SeriesCollectionControl.ActualYPlottingRangeChanged += SeriesCollectionControl_ActualYPlottingRangeChanged;

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

            this.PART_LegendHolder = (ContentControl)GetTemplateChild(sPART_LegendHolder);
            OnLegendChanged();
        }

        private void SeriesCollectionControl_ActualYPlottingRangeChanged(Range obj)
        {
            this.TryUpdatePlottingSettings(AxisType.Y);
        }

        private void SeriesCollectionControl_ActualXPlottingRangeChanged(Range obj)
        {
            this.TryUpdatePlottingSettings(AxisType.X);
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
            ((Chart)d).Part_SeriesCollectionControl?.UpdateActualPlottingXValueRange();
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
            ((Chart)d).Part_SeriesCollectionControl?.UpdateActualPlottingYValueRange();
        }
        #endregion

        #region SeriesDataTemplate & SeriesTemplateSelector
        public DataTemplate SeriesTemplate
        {
            get { return (DataTemplate)GetValue(SeriesTemplateProperty); }
            set { SetValue(SeriesTemplateProperty, value); }
        }

        public static readonly DependencyProperty SeriesTemplateProperty =
            DependencyProperty.Register("SeriesTemplate", typeof(DataTemplate), typeof(Chart), new PropertyMetadata(null, OnSeriesTemplatePropertyChanged));

        private static void OnSeriesTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).Part_SeriesCollectionControl?.OnSeriesTemplateChanged();
        }

        public DataTemplateSelector SeriesTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SeriesTemplateSelectorProperty); }
            set { SetValue(SeriesTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty SeriesTemplateSelectorProperty =
            DependencyProperty.Register("SeriesTemplateSelector", typeof(DataTemplateSelector), typeof(Chart), new PropertyMetadata(null, OnSeriesTemplateSelectorPropertyChanged));

        private static void OnSeriesTemplateSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).Part_SeriesCollectionControl?.OnSeriesTemplateSelectorChanged();
        }
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
            DependencyProperty.Register("SeriesItemsSource", typeof(IList), typeof(Chart), new PropertyMetadata(null, OnSeriesItemsSourcePropertyChanged));

        private static void OnSeriesItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).Part_SeriesCollectionControl?.OnSeriesItemsSourceChanged((IList)e.OldValue, (IList)e.NewValue);
        }

 

        #endregion

        #region SeriesStackMode
        public StackMode SeriesStackMode
        {
            get { return (StackMode)GetValue(SeriesStackModeProperty); }
            set { SetValue(SeriesStackModeProperty, value); }
        }
        public static readonly DependencyProperty SeriesStackModeProperty =
            DependencyProperty.Register("SeriesStackMode", typeof(StackMode), typeof(Chart), new PropertyMetadata(StackMode.NotStacked, OnSeriesStackModePropertyChanged));

        private static void OnSeriesStackModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).OnSeriesStackModeChanged();
        }

        private void OnSeriesStackModeChanged()
        {
            VerifyAccess();
 

            this.Part_SeriesCollectionControl?.OnStackModeChanged();
 

        }


        #endregion

        #region XValuePadding & YValuePadding
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
            DependencyProperty.Register("XValuePadding", typeof(Point), typeof(Chart), new PropertyMetadata(new Point(0, 0), OnPlottingXValuePaddingPropertyChanged));

        private static void OnPlottingXValuePaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).Part_SeriesCollectionControl?.OnXValuePaddingChanged();
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
            ((Chart)d).Part_SeriesCollectionControl?.OnYValuePaddingChanged();
        }

 
        #endregion

        #region PlottingSettings
        public event Action<PlottingRangeBase> HorizontalSettingChanged;
        public event Action<PlottingRangeBase> VerticalSettingChanged;

        private PlottingRangeBase _horizontalPlottingRangeSetting;
        private PlottingRangeBase _verticalPlottingRangeSetting;

        private PlottingRangeBase GetPlottingSettings(AxisType orientation)
        {
            if (this.Part_SeriesCollectionControl == null)
            {

                return null;
            }

            Range plottingDataRange, valuePadding;
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
                    plottingDataRange = this.Part_SeriesCollectionControl.XPlottingRange.ActualRange;

                    var sr = this.Part_SeriesCollectionControl.GetSeries().FirstOrDefault();

                    if (sr?.ItemsSource == null)
                    {
                        return null;
                    }
                    plottingItemValues = sr.ItemsSource.OfType<object>().Select(x => sr.GetXPropertyObjectForItem(x))
                        .ToArray();
                    break;


                case AxisType.Y:
                    if (this.YAxis == null)
                    {
                        return null;
                    }
                    valuePadding = new Range(this.YValuePadding.X, this.YValuePadding.Y);
                    isCategory = this.YAxis is ICategoryAxis;
                    plottingDataRange = this.Part_SeriesCollectionControl.YPlottingRange.ActualRange;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

            if (!plottingDataRange.IsInvalid)
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
                    return new NumericPlottingRange(plottingDataRange, valuePadding);
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

        #region BackgroundElement
        /// <summary>
        /// Represents pluggable background UIElement.
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

        #region IsSeriesCollectionChanging
        /// <summary>
        /// When collection or chart setting is changing, set this to true can
        /// reduce some performance-hitting, duplicated operation. 
        /// </summary>
        public bool IsSeriesCollectionChanging
        {
            get { return (bool)GetValue(IsSeriesCollectionChangingProperty); }
            set { SetValue(IsSeriesCollectionChangingProperty, value); }
        }
        public static readonly DependencyProperty IsSeriesCollectionChangingProperty =
            DependencyProperty.Register("IsSeriesCollectionChanging", typeof(bool), typeof(Chart), new PropertyMetadata(false, OnIsSeriesCollectionChangingPropertyChanged));

        private static void OnIsSeriesCollectionChangingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).OnIsSeriesCollectionChangingChanged();
        }

        private void OnIsSeriesCollectionChangingChanged()
        {
            VerifyAccess();
            if (this.Part_SeriesCollectionControl != null)
            {
                this.Part_SeriesCollectionControl.IsSeriesCollectionChanging = this.IsSeriesCollectionChanging;

            }
        }
        #endregion

        public double YBaseValue
        {
            get { return (double)GetValue(YBaseValueProperty); }
            set { SetValue(YBaseValueProperty, value); }
        }
        public static readonly DependencyProperty YBaseValueProperty =
            DependencyProperty.Register("YBaseValue", typeof(double), typeof(Chart), new PropertyMetadata(0d, OnYBaseValuePropertyChanged));

        private static void OnYBaseValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chart)d).Part_SeriesCollectionControl?.UpdateYBaseValue();
        }


        /// <summary>
        /// Called when x-axis or y-axis has updated the coordinates of its items.
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="ticks"></param>
        public void OnAxisItemsCoordinateChanged(AxisType orientation, IEnumerable<double> ticks)
        {
            this.GridLineControl?.OnAxisItemCoordinateChanged(orientation, ticks);
        }


    }


}
