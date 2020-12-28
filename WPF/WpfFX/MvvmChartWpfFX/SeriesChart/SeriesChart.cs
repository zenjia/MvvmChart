using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using MvvmCharting.Common;
using MvvmCharting.Axis;
using MvvmCharting.Drawing;
using MvvmCharting.GridLine;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// A Cartesian 2D Chart, which can displays a list of series(with <see cref="Scatter2"/>s).
    /// This is the host for almost everything: series plotting area,
    /// x axis & y axis, grid lines, cross hair...
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_PlottingCanvas", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_SeriesItemsControl", Type = typeof(SlimItemsControl))]
    [TemplatePart(Name = "PART_HorizontalCrossHair", Type = typeof(Line))]
    [TemplatePart(Name = "PART_VerticalCrossHair", Type = typeof(Line))]
    [TemplatePart(Name = "PART_GridLineHolder", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_LegendHolder", Type = typeof(ContentControl))]

    public class SeriesChart : Control, IXAxisOwner, IYAxisOwner
    {
        static SeriesChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SeriesChart), new FrameworkPropertyMetadata(typeof(SeriesChart)));
        }

        private static readonly string sPART_Root = "PART_Root";
        private static readonly string sPART_PlottingCanvas = "PART_PlottingCanvas";

        private static readonly string sPART_SeriesItemsControl = "PART_SeriesItemsControl";

        private static readonly string sPART_HorizontalCrossHair = "PART_HorizontalCrossHair";
        private static readonly string sPART_VerticalCrossHair = "PART_VerticalCrossHair";

        private static readonly string sPART_GridLineHolder = "PART_GridLineHolder";
        private static readonly string sPART_LegendHolder = "PART_LegendHolder";

        private Grid PART_Root;
        private Grid PART_PlottingCanvas;
        private SlimItemsControl PART_SeriesItemsControl;

        private Line PART_HorizontalCrossHair;
        private Line PART_VerticalCrossHair;

        private ContentControl PART_GridLineHolder;
        private ContentControl PART_LegendHolder;

        private int SeriesCount => this.PART_SeriesItemsControl?.ItemCount ?? 0;
        private IEnumerable<ISeries> GetSeries()
        {
            if (this.PART_SeriesItemsControl == null)
            {
                return Enumerable.Empty<ISeries>();
            }

            return this.PART_SeriesItemsControl.GetAllElements().OfType<ISeries>();
        }

        public SeriesChart()
        {

        }

        #region overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            if (this.PART_SeriesItemsControl != null)
            {
                this.PART_SeriesItemsControl.ElementGenerated -= SeriesItemTemplateApplied;
            }

            this.PART_SeriesItemsControl = (SlimItemsControl)GetTemplateChild(sPART_SeriesItemsControl);

            if (this.PART_SeriesItemsControl != null)
            {
                this.PART_SeriesItemsControl.ElementGenerated += SeriesItemTemplateApplied;

                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemTemplateProperty,
                    new Binding(nameof(this.SeriesDataTemplate)) { Source = this });
                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemTemplateSelectorProperty,
                    new Binding(nameof(this.SeriesTemplateSelector)) { Source = this });
                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemsSourceProperty,
                    new Binding(nameof(this.SeriesItemsSource)) { Source = this });
            }




            this.PART_Root = (Grid)GetTemplateChild(sPART_Root);
            this.PART_PlottingCanvas = (Grid)GetTemplateChild(sPART_PlottingCanvas);

            OnXAxisPropertyChanged(null, this.XAxis);
            OnYAxisPropertyChanged(null, this.YAxis);
            OnBackgroundImageChanged(null, this.BackgroundImage);

            this.PART_HorizontalCrossHair = (Line)GetTemplateChild(sPART_HorizontalCrossHair);
            if (this.PART_HorizontalCrossHair != null)
            {
                this.PART_HorizontalCrossHair.SetBinding(Control.StyleProperty,
                    new Binding(nameof(HorizontalCrossHairLineStyle)) { Source = this });
                this.PART_HorizontalCrossHair.SetBinding(Line.X2Property,
                    new Binding(nameof(ActualWidth)) { Source = this.PART_PlottingCanvas });

            }

            this.PART_VerticalCrossHair = (Line)GetTemplateChild(sPART_VerticalCrossHair);
            if (this.PART_VerticalCrossHair != null)
            {
                this.PART_VerticalCrossHair.SetBinding(Control.StyleProperty,
                    new Binding(nameof(VerticalCrossHairLineStyle)) { Source = this });
                this.PART_VerticalCrossHair.SetBinding(Line.Y2Property,
                    new Binding(nameof(ActualHeight)) { Source = this.PART_PlottingCanvas });
            }

            this.PART_GridLineHolder = (ContentControl)GetTemplateChild(sPART_GridLineHolder);


            OnGridLineControlChanged();

            this.PART_PlottingCanvas.MouseMove += PartPlottingCanvasMouseMove;
            this.PART_PlottingCanvas.MouseLeave += PartPlottingCanvasMouseLeave;
            this.PART_PlottingCanvas.SizeChanged += PartPlottingCanvasSizeChanged;

            this.PART_LegendHolder = (ContentControl)GetTemplateChild(sPART_LegendHolder);
            OnLegendChanged();
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
        private void SeriesItemTemplateApplied(object sender, DependencyObject root)
        {

            if (root == null)
            {
                return;
            }

            var sr = root as ISeries;
            if (sr == null)
            {
                throw new MvvmChartException("The root element in the SeriesDataTemplate should implement ISeries!");
            }


            sr.XRangeChanged += Sr_XRangeChanged;
            sr.YRangeChanged += Sr_YRangeChanged;
            sr.PropertyChanged += Sr_PropertyChanged;

            UpdateGlobalDataRange();

            OnPlottingYDataRangeChanged();
            OnPlottingXDataRangeChanged();

        }

        private void Sr_PropertyChanged(object sender, string propertyName)
        {
            var sr = (ISeries)sender;
            if (propertyName == nameof(sr.IsHighLighted))
            {
                this.Legend.OnItemHighlightChanged(sr.DataContext, sr.IsHighLighted);
            }

        }

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
            DependencyProperty.Register("XMinimum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN, OnXMinimumOrXMaximumPropertyChanged));

        /// <summary>
        /// The maximum independent value should be plotted.
        /// </summary>
        public double XMaximum
        {
            get { return (double)GetValue(XMaximumProperty); }
            set { SetValue(XMaximumProperty, value); }
        }
        public static readonly DependencyProperty XMaximumProperty =
            DependencyProperty.Register("XMaximum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN, OnXMinimumOrXMaximumPropertyChanged));

        private static void OnXMinimumOrXMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).UpdatePlottingXDataRange();
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
            DependencyProperty.Register("YMinimum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN, OnYMinimumOrYMaximumPropertyChanged));

        /// <summary>
        /// The maximum dependent value should be plotted.
        /// </summary>
        public double YMaximum
        {
            get { return (double)GetValue(YMaximumProperty); }
            set { SetValue(YMaximumProperty, value); }
        }
        public static readonly DependencyProperty YMaximumProperty =
            DependencyProperty.Register("YMaximum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN));

        private static void OnYMinimumOrYMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).UpdatePlottingYDataRange();
        }
        #endregion

        #region SeriesDataTemplate & SeriesTemplateSelector
        public DataTemplate SeriesDataTemplate
        {
            get { return (DataTemplate)GetValue(SeriesDataTemplateProperty); }
            set { SetValue(SeriesDataTemplateProperty, value); }
        }
        public static readonly DependencyProperty SeriesDataTemplateProperty =
            DependencyProperty.Register("SeriesDataTemplate", typeof(DataTemplate), typeof(SeriesChart), new PropertyMetadata(null));

        public DataTemplateSelector SeriesTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SeriesTemplateSelectorProperty); }
            set { SetValue(SeriesTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty SeriesTemplateSelectorProperty =
            DependencyProperty.Register("SeriesTemplateSelector", typeof(DataTemplateSelector), typeof(SeriesChart), new PropertyMetadata(null));
        #endregion

        #region SeriesItemsSource
        /// <summary>
        /// Represents the data for a list of series(<see cref="SeriesBase"/>). 
        /// </summary>
        public IList SeriesItemsSource
        {
            get { return (IList)GetValue(SeriesItemsSourceProperty); }
            set { SetValue(SeriesItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty SeriesItemsSourceProperty =
            DependencyProperty.Register("SeriesItemsSource", typeof(IList), typeof(SeriesChart), new PropertyMetadata(null, OnSeriesItemsSourceChanged));

        private static void OnSeriesItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnSeriesItemsSourceChanged((IList)e.OldValue, (IList)e.NewValue);
        }

        private void OnSeriesItemsSourceChanged(IList oldValue, IList newValue)
        {
            if (oldValue is INotifyCollectionChanged oldItemsSource)
            {
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .RemoveHandler(oldItemsSource, "CollectionChanged", SeriesItemsSource_CollectionChanged);
            }

            if (newValue is INotifyCollectionChanged newItemsSource)
            {
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .AddHandler(newItemsSource, "CollectionChanged", SeriesItemsSource_CollectionChanged);
            }


            if (this.Legend != null)
            {
                this.Legend.ItemsSource = this.SeriesItemsSource;
            }
        }

        private void SeriesItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == NotifyCollectionChangedAction.Move)
            //{
            //    return;
            //}

            //if (e.Action == NotifyCollectionChangedAction.Reset)
            //{
            //    this._seriesDictionary.Clear();
            //}

            //if (e.OldItems != null)
            //{
            //    foreach (var oldItem in e.OldItems)
            //    {
            //        this._seriesDictionary.Remove(oldItem);
            //    }
            //}
        }
        #endregion

        #region Global Data Range
        private Range _globalYDataRange = Range.Empty;
        /// <summary>
        /// The dependent value Range(min & max) of all series data
        /// </summary>
        public Range GlobalYDataRange
        {
            get { return this._globalYDataRange; }
            set
            {
                if (this._globalYDataRange != value)
                {
                    this._globalYDataRange = value;

                    UpdatePlottingYDataRange();
                }
            }
        }

        private Range _globalXDataRange = Range.Empty;
        /// <summary>
        /// The independent value Range(min & max) of all series data
        /// </summary>
        public Range GlobalXDataRange
        {
            get { return this._globalXDataRange; }
            set
            {
                if (this._globalXDataRange != value)
                {
                    this._globalXDataRange = value;

                    UpdatePlottingXDataRange();
                }
            }
        }

        private void Sr_XRangeChanged(Range obj)
        {
            UpdateGlobalDataRange();
        }

        private void Sr_YRangeChanged(Range obj)
        {
            UpdateGlobalDataRange();
        }

        private void UpdateGlobalDataRange()
        {
            if (this.SeriesCount == 0)
            {
                this.GlobalXDataRange = Range.Empty;
                this.GlobalYDataRange = Range.Empty;
                return;
            }

            double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;

            bool isXDataRangeEmplty = true;
            bool isYDataRangeEmplty = true;
            foreach (var sr in this.GetSeries())
            {

                if (!sr.XDataRange.IsEmpty)
                {
                    minX = Math.Min(minX, sr.XDataRange.Min);
                    maxX = Math.Max(maxX, sr.XDataRange.Max);

                    if (isXDataRangeEmplty)
                    {
                        isXDataRangeEmplty = false;
                    }

                }

                if (!sr.YDataRange.IsEmpty)
                {
                    minY = Math.Min(minY, sr.YDataRange.Min);
                    maxY = Math.Max(maxY, sr.YDataRange.Max);
                    if (isYDataRangeEmplty)
                    {
                        isYDataRangeEmplty = false;
                    }
                }

            }



            if (!isXDataRangeEmplty)
            {
                this.GlobalXDataRange = new Range(minX, maxX);
            }

            if (!isYDataRangeEmplty)
            {
                this.GlobalYDataRange = new Range(minY, maxY);
            }


        }
        #endregion

        #region Plotting Data Range
        private Range _plottingXDataRange = Range.Empty;
        /// <summary>
        /// The final independent value range(min & max) used to plot chart
        /// </summary>
        public Range PlottingXDataRange
        {
            get
            {

                return this._plottingXDataRange;
            }
            set
            {
                if (this._plottingXDataRange != value)
                {
                    this._plottingXDataRange = value;
                    this.PlottingXRangeChanged?.Invoke(value);
                    TryUpdatePlottingSettings(AxisType.X);

                    OnPlottingXDataRangeChanged();
                }
            }
        }

        private Range _plottingYDataRange = Range.Empty;
        /// <summary>
        /// The final dependent value range(min & max) used to plot chart
        /// </summary>
        public Range PlottingYDataRange
        {
            get { return this._plottingYDataRange; }
            set
            {
                if (this._plottingYDataRange != value)
                {
                    this._plottingYDataRange = value;
                    this.PlottingYRangeChanged?.Invoke(value);


                    TryUpdatePlottingSettings(AxisType.Y);
                    OnPlottingYDataRangeChanged();
                }
            }
        }

        private void UpdatePlottingXDataRange()
        {
            double min = !this.XMinimum.IsNaN() ? this.XMinimum : this.GlobalXDataRange.Min;
            double max = !this.XMaximum.IsNaN() ? this.XMaximum : this.GlobalXDataRange.Max;

            if (min.IsNaN() && max.IsNaN())
            {
                this.PlottingXDataRange = Range.Empty;
                return;
            }

            if (this.PlottingXDataRange.IsEmpty ||
                !this.PlottingXDataRange.Min.NearlyEqual(min) ||
                !this.PlottingXDataRange.Max.NearlyEqual(max))
            {

                this.PlottingXDataRange = new Range(min, max);


            }
        }

        private void UpdatePlottingYDataRange()
        {
            double min = !this.YMinimum.IsNaN() ? this.YMinimum : this.GlobalYDataRange.Min;
            double max = !this.YMaximum.IsNaN() ? this.YMaximum : this.GlobalYDataRange.Max;

            if (min.IsNaN() && max.IsNaN())
            {
                this.PlottingYDataRange = Range.Empty;
                return;
            }

            if (this.PlottingYDataRange.IsEmpty ||
                this.PlottingYDataRange.Min != min ||
                this.PlottingYDataRange.Max != max)
            {

                this.PlottingYDataRange = new Range(min, max);
            }
        }

        private void OnPlottingXDataRangeChanged()
        {
            foreach (var sr in this.GetSeries())
            {
                sr.PlottingXDataRange = this.PlottingXDataRange;
            }
        }

        private void OnPlottingYDataRangeChanged()
        {
            foreach (var sr in this.GetSeries())
            {
                sr.PlottingYDataRange = this.PlottingYDataRange;
            }
        }

        public event Action<Range> PlottingXRangeChanged;
        public event Action<Range> PlottingYRangeChanged;
        #endregion

        #region PlottingSettings
        public event Action<PlottingSettings> HorizontalSettingChanged;
        public event Action<PlottingSettings> VerticalSettingChanged;

        private PlottingSettings _plottingHorizontalSetting;
        public PlottingSettings PlottingHorizontalSetting
        {
            get { return this._plottingHorizontalSetting; }
            set
            {
                if (this._plottingHorizontalSetting != value)
                {
                    this._plottingHorizontalSetting = value;

                    this.HorizontalSettingChanged?.Invoke(value);
                }
            }
        }

        private PlottingSettings _plottingVerticalSetting;
        public PlottingSettings PlottingVerticalSetting
        {
            get { return this._plottingVerticalSetting; }
            set
            {
                if (this._plottingVerticalSetting != value)
                {
                    this._plottingVerticalSetting = value;
                    this.VerticalSettingChanged?.Invoke(value);
                }
            }
        }
        private PlottingSettings GetPlottingSettings(AxisType orientation)
        {
            if (this.PART_PlottingCanvas == null)
            {

                return null;
            }

            double length;
            PointNS magrin, pading, borderThickness;
            Range plotingDataRange;
            switch (orientation)
            {
                case AxisType.X:

                    length = this.PART_PlottingCanvas.ActualWidth;
                    magrin = new PointNS(this.Margin.Left, this.Margin.Right);
                    pading = new PointNS(this.Padding.Left, this.Padding.Right);
                    borderThickness = new PointNS(this.BorderThickness.Left, this.BorderThickness.Right);
                    plotingDataRange = this.PlottingXDataRange;
                    break;


                case AxisType.Y:
                    length = this.PART_PlottingCanvas.ActualHeight;
                    magrin = new PointNS(this.Margin.Top, this.Margin.Bottom);
                    pading = new PointNS(this.Padding.Top, this.Padding.Bottom);
                    borderThickness = new PointNS(this.BorderThickness.Top, this.BorderThickness.Bottom);
                    plotingDataRange = this.PlottingYDataRange;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

            var isValid = PlottingSettings.Validate(length, magrin, pading, borderThickness, plotingDataRange);

            if (isValid)
            {
                return new PlottingSettings(orientation, length, magrin, pading, borderThickness, plotingDataRange);
            }

            return null;
        }
        private void TryUpdatePlottingSettings(AxisType orientation)
        {
            if (!this.IsLoaded)
            {
                return;
            }


            var args = GetPlottingSettings(orientation);
            switch (orientation)
            {
                case AxisType.X:
                    this.PlottingHorizontalSetting = args;
                    break;
                case AxisType.Y:
                    this.PlottingVerticalSetting = args;
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
            DependencyProperty.Register("GridLineControl", typeof(IGridLineControl), typeof(SeriesChart), new PropertyMetadata(null, OnGridLineControlPropertyChanged));

        private static void OnGridLineControlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnGridLineControlChanged();
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
            DependencyProperty.Register("GridLineControlVisibility", typeof(Visibility), typeof(SeriesChart), new PropertyMetadata(Visibility.Visible));


        #endregion

        #region Axises
        public IAxisNS XAxis
        {
            get { return (IAxisNS)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(IAxisNS), typeof(SeriesChart), new PropertyMetadata(null, OnXAxisPropertyChanged));

        private static void OnXAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnXAxisPropertyChanged((IAxisNS)e.OldValue, (IAxisNS)e.NewValue);
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
            DependencyProperty.Register("YAxis", typeof(IAxisNS), typeof(SeriesChart), new PropertyMetadata(null, OnYAxisPropertyChanged));
        private static void OnYAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnYAxisPropertyChanged((IAxisNS)e.OldValue, (IAxisNS)e.NewValue);
        }
        private void OnYAxisPropertyChanged(IAxisNS oldValue, IAxisNS newValue)
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
        public UIElement BackgroundImage
        {
            get { return (UIElement)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }
        public static readonly DependencyProperty BackgroundImageProperty =
            DependencyProperty.Register("BackgroundImage", typeof(UIElement), typeof(SeriesChart), new PropertyMetadata(null, OnBackgroundImagePropertyChanged));

        private static void OnBackgroundImagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ((SeriesChart)d).OnBackgroundImageChanged((UIElement)e.OldValue, (UIElement)e.NewValue);
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
            DependencyProperty.Register("HorizontalCrossHairVisibility", typeof(Visibility), typeof(SeriesChart), new PropertyMetadata(Visibility.Visible));


        public Visibility VerticalCrossHairVisiblity
        {
            get { return (Visibility)GetValue(VerticalCrossHairVisiblityProperty); }
            set { SetValue(VerticalCrossHairVisiblityProperty, value); }
        }
        public static readonly DependencyProperty VerticalCrossHairVisiblityProperty =
            DependencyProperty.Register("VerticalCrossHairVisiblity", typeof(Visibility), typeof(SeriesChart), new PropertyMetadata(Visibility.Visible));




        public Style HorizontalCrossHairLineStyle
        {
            get { return (Style)GetValue(HorizontalCrossHairLineStyleProperty); }
            set { SetValue(HorizontalCrossHairLineStyleProperty, value); }
        }
        public static readonly DependencyProperty HorizontalCrossHairLineStyleProperty =
            DependencyProperty.Register("HorizontalCrossHairLineStyle", typeof(Style), typeof(SeriesChart), new PropertyMetadata(null));


        public Style VerticalCrossHairLineStyle
        {
            get { return (Style)GetValue(VerticalCrossHairLineStyleProperty); }
            set { SetValue(VerticalCrossHairLineStyleProperty, value); }
        }
        public static readonly DependencyProperty VerticalCrossHairLineStyleProperty =
            DependencyProperty.Register("VerticalCrossHairLineStyle", typeof(Style), typeof(SeriesChart), new PropertyMetadata(null));



        private void PartPlottingCanvasMouseMove(object sender, MouseEventArgs e)
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
                MoveCrossHairLine(this.PART_HorizontalCrossHair, mousePoint.Y);
            }

            if (isVerticalCrossHairVisible)
            {
                MoveCrossHairLine(this.PART_VerticalCrossHair, mousePoint.X);
            }


        }

        private void PartPlottingCanvasMouseLeave(object sender, MouseEventArgs e)
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

        private void MoveCrossHairLine(Line crossHairLine, double offset)
        {

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
            DependencyProperty.Register("Legend", typeof(LegendControl), typeof(SeriesChart), new PropertyMetadata(null, OnLegendPropertyChanged));

        private static void OnLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnLegendChanged();
        }

        private void OnLegendChanged()
        {
            if (this.Legend != null)
            {
                this.Legend.ItemsSource = this.SeriesItemsSource;
                this.Legend.LegendItemTemplate = this.LegendItemTemplate;
                this.Legend.LegendItemHighlighChanged += Legend_LegendItemHighlighChanged;
            }

            if (this.PART_LegendHolder != null)
            {
                this.PART_LegendHolder.Content = this.Legend;


            }
        }

        private void Legend_LegendItemHighlighChanged(LegendItemControl sender, bool newValue)
        {
            var item = sender.DataContext;
            var sr = this.PART_SeriesItemsControl?.TryGetElementForItem(item) as ISeries;

            if (sr != null)
            {
                sr.IsHighLighted = newValue;
            }
        }

        public DataTemplate LegendItemTemplate
        {
            get { return (DataTemplate)GetValue(LegendItemTemplateProperty); }
            set { SetValue(LegendItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty LegendItemTemplateProperty =
            DependencyProperty.Register("LegendItemTemplate", typeof(DataTemplate), typeof(SeriesChart), new PropertyMetadata(null, OnLegendItemTemplatePropertyChanged));

        private static void OnLegendItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnLegendItemTemplateChanged();
        }

        private void OnLegendItemTemplateChanged()
        {
            if (this.Legend != null)
            {
                this.Legend.LegendItemTemplate = this.LegendItemTemplate;
            }
        }




        public Visibility LegendVisibility
        {
            get { return (Visibility)GetValue(LegendVisibilityProperty); }
            set { SetValue(LegendVisibilityProperty, value); }
        }
        public static readonly DependencyProperty LegendVisibilityProperty =
            DependencyProperty.Register("LegendVisibility", typeof(Visibility), typeof(SeriesChart), new PropertyMetadata(Visibility.Visible));



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
    }
}
