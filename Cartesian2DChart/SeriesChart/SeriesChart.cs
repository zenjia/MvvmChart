using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MvvmChart.Common;
using MvvmCharting.Axis;

namespace MvvmCharting
{


    public class CanvasSettingChangedEventArgs
    {
        public override string ToString()
        {
            return "aaa";
        }

        public Orientation Orientation { get; }
        public double RenderSize { get; }

        public Point Margin { get; }
        public Point Padding { get; }
        public Point BorderThickness { get; }

        public Range PlotingDataRange { get; }

        public double GetAvailablePlottingSize()
        {
            return this.RenderSize - (this.Margin.X + this.Margin.Y)
                   - (this.Padding.X + this.Padding.Y) -
                   (this.BorderThickness.X + this.BorderThickness.Y);
        }

        public CanvasSettingChangedEventArgs(Orientation orientation,
            double renderSize,
            Point margin,
            Point padding,
            Point borderThickness,
            Range plotingDataRange)
        {
            this.RenderSize = renderSize;
            this.Margin = margin;
            this.Padding = padding;
            this.BorderThickness = borderThickness;
            this.PlotingDataRange = plotingDataRange;
            this.Orientation = orientation;
        }

        public override bool Equals(object obj)
        {
            var other = obj as CanvasSettingChangedEventArgs;
            if (other == null)
            {
                return false;
            }
            return other.Equals(this);
        }

        public bool Equals(CanvasSettingChangedEventArgs obj)
        {
            return this.RenderSize.NearlyEqual(obj.RenderSize, 0.0001) &&
                   this.Margin == obj.Margin &&
                   this.Padding == obj.Padding &&
                   this.BorderThickness == obj.BorderThickness &&
                   this.PlotingDataRange == obj.PlotingDataRange;
        }

        public static bool Validate(double length,
            Point margin,
            Point pading,
            Point borderThickness,
            Range plotingDataRange)
        {
            return !length.IsInvalid() &&
                   !length.IsZero() &&
                   !margin.IsInvalid() &&
                   !pading.IsInvalid() &&
                   !borderThickness.IsInvalid() &&
                   !plotingDataRange.IsInvalid;

        }
    }

    public interface IAxisOwner
    {
        void OnAxisItemsCoordinateChanged(Orientation orientation, IEnumerable<double> ticks);
    }

    public interface IXAxisOwner : IAxisOwner
    {


        event Action<CanvasSettingChangedEventArgs> CanvasHorizontalSettingChanged;


    }

    public interface IYAxisOwner : IAxisOwner
    {


        event Action<CanvasSettingChangedEventArgs> CanvasVerticalSettingChanged;
    }


    /// <summary>
    /// A Cartesian 2D Chart, which can displays a list of series(with item points).
    /// This is the host for almost everything: series plot area,
    /// x axis & y axis, grid lines, cross hair...
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_PlotingCanvas", Type = typeof(Grid))]

    [TemplatePart(Name = "PART_SeriesItemsControl", Type = typeof(SlimItemsControl))]
    [TemplatePart(Name = "PART_HorizontalCrossHair", Type = typeof(Line))]
    [TemplatePart(Name = "PART_VerticalCrossHair", Type = typeof(Line))]
    [TemplatePart(Name = "PART_GridLineHolder", Type = typeof(Line))]

    public class SeriesChart : Control, IXAxisOwner, IYAxisOwner
    {
        static SeriesChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SeriesChart), new FrameworkPropertyMetadata(typeof(SeriesChart)));
        }

        private static readonly string sPART_Root = "PART_Root";
        private static readonly string sPART_PlotingCanvas = "PART_PlotingCanvas";

        private static readonly string sPART_SeriesItemsControl = "PART_SeriesItemsControl";
        private static readonly string sPART_HorizontalCrossHair = "PART_HorizontalCrossHair";
        private static readonly string sPART_VerticalCrossHair = "PART_VerticalCrossHair";
        private static readonly string sPART_GridLineHolder = "PART_GridLineHolder";


        public event Action<Range> PlotingXRangeChanged;
        public event Action<Range> PlotingYRangeChanged;

        public event Action<CanvasSettingChangedEventArgs> CanvasHorizontalSettingChanged;
        public event Action<CanvasSettingChangedEventArgs> CanvasVerticalSettingChanged;

        private CanvasSettingChangedEventArgs _canvasHorizontalSetting;
        public CanvasSettingChangedEventArgs CanvasHorizontalSetting
        {
            get { return this._canvasHorizontalSetting; }
            set
            {
                if (this._canvasHorizontalSetting != value)
                {
                    this._canvasHorizontalSetting = value;

                    this.CanvasHorizontalSettingChanged?.Invoke(value);
                }
            }
        }

        private CanvasSettingChangedEventArgs _canvasVerticalSetting;
        public CanvasSettingChangedEventArgs CanvasVerticalSetting
        {
            get { return this._canvasVerticalSetting; }
            set
            {
                if (this._canvasVerticalSetting != value)
                {
                    this._canvasVerticalSetting = value;
                    this.CanvasVerticalSettingChanged?.Invoke(value);
                }
            }
        }
        private CanvasSettingChangedEventArgs GetCanvasSettingChangedEventArgs(Orientation orientation)
        {
            if (this.PART_PlotingCanvas == null)
            {

                return null;
            }

            double length;
            Point magrin, pading, borderThickness;
            Range plotingDataRange;
            switch (orientation)
            {
                case Orientation.Horizontal:

                    length = this.PART_PlotingCanvas.ActualWidth;
                    magrin = new Point(this.Margin.Left, this.Margin.Right);
                    pading = new Point(this.Padding.Left, this.Padding.Right);
                    borderThickness = new Point(this.BorderThickness.Left, this.BorderThickness.Right);
                    plotingDataRange = this.PlotingXDataRange;
                    break;


                case Orientation.Vertical:
                    length = this.PART_PlotingCanvas.ActualHeight;
                    magrin = new Point(this.Margin.Top, this.Margin.Bottom);
                    pading = new Point(this.Padding.Top, this.Padding.Bottom);
                    borderThickness = new Point(this.BorderThickness.Top, this.BorderThickness.Bottom);
                    plotingDataRange = this.PlotingYDataRange;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

            var isValid = CanvasSettingChangedEventArgs.Validate(length, magrin, pading, borderThickness, plotingDataRange);

            if (isValid)
            {
                return new CanvasSettingChangedEventArgs(orientation, length, magrin, pading, borderThickness, plotingDataRange);
            }

            return null;
        }
        private void DetectCanvasSettingChanged(Orientation orientation)
        {
            if (!this.IsLoaded)
            {
                return;
            }


            var args = GetCanvasSettingChangedEventArgs(orientation);
            switch (orientation)
            {
                case Orientation.Horizontal:
                    this.CanvasHorizontalSetting = args;
                    break;
                case Orientation.Vertical:
                    this.CanvasVerticalSetting = args;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

        }

        private Grid PART_Root;
        private Grid PART_PlotingCanvas;
        private SlimItemsControl PART_SeriesItemsControl;

        private Line PART_HorizontalCrossHair;
        private Line PART_VerticalCrossHair;

        private ContentControl PART_GridLineHolder;

        private Dictionary<object, SeriesBase> _seriesDictionary = new Dictionary<object, SeriesBase>();
        public SeriesChart()
        {

        }

        #region overrides



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            if (this.PART_SeriesItemsControl != null)
            {
                this.PART_SeriesItemsControl.ItemTemplateContentLoaded -= SeriesItemTemplateApplied;
            }

            this.PART_SeriesItemsControl = (SlimItemsControl)GetTemplateChild(sPART_SeriesItemsControl);
            this.PART_SeriesItemsControl.ItemTemplateContentLoaded += SeriesItemTemplateApplied;



            this.PART_Root = (Grid)GetTemplateChild(sPART_Root);
            this.PART_PlotingCanvas = (Grid)GetTemplateChild(sPART_PlotingCanvas);

            OnXAxisPropertyChanged(null, this.XAxis);
            OnYAxisPropertyChanged(null, this.YAxis);
            OnPlotAreaBackgroundPropertyChanged(null, this.PlotAreaBackground);

            this.PART_HorizontalCrossHair = (Line)GetTemplateChild(sPART_HorizontalCrossHair);
            if (this.PART_HorizontalCrossHair != null)
            {
                this.PART_HorizontalCrossHair.SetBinding(Control.StyleProperty,
                    new Binding(nameof(HorizontalCrossHairLineStyle)){Source = this});
                this.PART_HorizontalCrossHair.SetBinding(Line.Y2Property,
                    new Binding(nameof(ActualHeight)) { Source = this.PART_PlotingCanvas });
            }

            this.PART_VerticalCrossHair = (Line)GetTemplateChild(sPART_VerticalCrossHair);
            if (this.PART_VerticalCrossHair != null)
            {
                this.PART_VerticalCrossHair.SetBinding(Control.StyleProperty,
                    new Binding(nameof(VerticalCrossHairLineStyle)) { Source = this });
                this.PART_VerticalCrossHair.SetBinding(Line.X2Property,
                    new Binding(nameof(ActualHeight)) { Source = this.PART_PlotingCanvas });
            }

            this.PART_GridLineHolder = (ContentControl)GetTemplateChild(sPART_GridLineHolder);
            OnGridLineControlChanged();

            this.PART_PlotingCanvas.MouseMove += PartPlotingCanvasMouseMove;
            this.PART_PlotingCanvas.MouseLeave += PartPlotingCanvasMouseLeave;
            this.PART_PlotingCanvas.SizeChanged += PART_PlotingCanvas_SizeChanged;
        }



        private void PART_PlotingCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (!this.IsLoaded)
            {
                return;
            }





            if (e.WidthChanged)
            {
                DetectCanvasSettingChanged(Orientation.Horizontal);
            }

            if (e.HeightChanged)
            {
                DetectCanvasSettingChanged(Orientation.Vertical);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == PaddingProperty)
            {
                DetectCanvasSettingChanged(Orientation.Horizontal);
                DetectCanvasSettingChanged(Orientation.Vertical);
            }
            else if (e.Property == MarginProperty)
            {
                DetectCanvasSettingChanged(Orientation.Horizontal);
                DetectCanvasSettingChanged(Orientation.Vertical);
            }
            else if (e.Property == BorderThicknessProperty)
            {
                DetectCanvasSettingChanged(Orientation.Horizontal);
                DetectCanvasSettingChanged(Orientation.Vertical);
            }
        }
        #endregion


        public double XMinimum
        {
            get { return (double)GetValue(XMinimumProperty); }
            set { SetValue(XMinimumProperty, value); }
        }
        public static readonly DependencyProperty XMinimumProperty =
            DependencyProperty.Register("XMinimum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN, OnXMinimumOrXMaximumPropertyChanged));

        public double XMaximum
        {
            get { return (double)GetValue(XMaximumProperty); }
            set { SetValue(XMaximumProperty, value); }
        }
        public static readonly DependencyProperty XMaximumProperty =
            DependencyProperty.Register("XMaximum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN, OnXMinimumOrXMaximumPropertyChanged));

        private static void OnXMinimumOrXMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).UpdatePlotAreaXDataRange();
        }


        public double YMinimum
        {
            get { return (double)GetValue(YMinimumProperty); }
            set { SetValue(YMinimumProperty, value); }
        }
        public static readonly DependencyProperty YMinimumProperty =
            DependencyProperty.Register("YMinimum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN, OnYMinimumOrYMaximumPropertyChanged));

        public double YMaximum
        {
            get { return (double)GetValue(YMaximumProperty); }
            set { SetValue(YMaximumProperty, value); }
        }
        public static readonly DependencyProperty YMaximumProperty =
            DependencyProperty.Register("YMaximum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN));


        private static void OnYMinimumOrYMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).UpdatePlotAreaYDataRange();
        }


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

        }

        private void SeriesItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this._seriesDictionary.Clear();
            }

            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    this._seriesDictionary.Remove(oldItem);
                }
            }
        }


        private Range _globalYDataRange = Range.Empty;

        public Range GlobalYDataRange
        {
            get { return this._globalYDataRange; }
            set
            {
                if (this._globalYDataRange != value)
                {
                    this._globalYDataRange = value;

                    UpdatePlotAreaYDataRange();
                }
            }
        }

        private Range _globalXDataRange = Range.Empty;

        public Range GlobalXDataRange
        {
            get { return this._globalXDataRange; }
            set
            {
                if (this._globalXDataRange != value)
                {
                    this._globalXDataRange = value;

                    UpdatePlotAreaXDataRange();
                }
            }
        }


        private void OnPlotingXDataRangeChanged()
        {
            foreach (var sr in this._seriesDictionary.Values)
            {
                sr.PlotingXDataRange = this.PlotingXDataRange;
            }
        }

        private void OnPlotingYDataRangeChanged()
        {
            foreach (var sr in this._seriesDictionary.Values)
            {
                sr.PlotingYDataRange = this.PlotingYDataRange;
            }
        }

        private void UpdateGlobalDataRange()
        {
            if (this._seriesDictionary.Count == 0)
            {
                this.GlobalXDataRange = Range.Empty;
                this.GlobalYDataRange = Range.Empty;
                return;
            }

            double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;

            bool isXDataRangeEmplty = true;
            bool isYDataRangeEmplty = true;
            foreach (var sr in this._seriesDictionary.Values)
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

        private Range _plotAreaXDataRange;
        public Range PlotingXDataRange
        {
            get
            {

                return this._plotAreaXDataRange;
            }
            set
            {
                if (this._plotAreaXDataRange != value)
                {
                    this._plotAreaXDataRange = value;
                    this.PlotingXRangeChanged?.Invoke(value);
                    DetectCanvasSettingChanged(Orientation.Horizontal);

                    OnPlotingXDataRangeChanged();
                }
            }
        }

        private Range _plotAreaYDataRange;
        public Range PlotingYDataRange
        {
            get { return this._plotAreaYDataRange; }
            set
            {
                if (this._plotAreaYDataRange != value)
                {
                    this._plotAreaYDataRange = value;
                    this.PlotingYRangeChanged?.Invoke(value);


                    DetectCanvasSettingChanged(Orientation.Vertical);
                    OnPlotingYDataRangeChanged();
                }
            }
        }

        private void UpdatePlotAreaXDataRange()
        {
            double min = !this.XMinimum.IsNaN() ? this.XMinimum : this.GlobalXDataRange.Min;
            double max = !this.XMaximum.IsNaN() ? this.XMaximum : this.GlobalXDataRange.Max;

            if (min.IsNaN() && max.IsNaN())
            {
                this.PlotingXDataRange = Range.Empty;
                return;
            }

            if (this.PlotingXDataRange.IsEmpty ||
                !this.PlotingXDataRange.Min.NearlyEqual(min) ||
                !this.PlotingXDataRange.Max.NearlyEqual(max))
            {

                this.PlotingXDataRange = new Range(min, max);


            }
        }

        private void UpdatePlotAreaYDataRange()
        {
            double min = !this.YMinimum.IsNaN() ? this.YMinimum : this.GlobalYDataRange.Min;
            double max = !this.YMaximum.IsNaN() ? this.YMaximum : this.GlobalYDataRange.Max;

            if (min.IsNaN() && max.IsNaN())
            {
                this.PlotingYDataRange = Range.Empty;
                return;
            }

            if (this.PlotingYDataRange.IsEmpty ||
                this.PlotingYDataRange.Min != min ||
                this.PlotingYDataRange.Max != max)
            {

                this.PlotingYDataRange = new Range(min, max);
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



        private void SeriesItemTemplateApplied(object sender, DependencyObject root)
        {

            if (root == null)
            {
                return;
            }

            var sr = root as SeriesBase;
            if (sr == null)
            {
                throw new MvvmChartException("The root element in the SeriesDataTemplate should be of type: SeriesBase!");
            }

            var item = sr.DataContext;

            //If the ItemTemplate or ItemTemplateSelector of an ItemsControl is replaced, then its
            //ItemContainer will re-apply its Template(i.e. ItemTemplate), which will regenerate
            //the TemplateChild of its ItemContainer. We should check this and remove the old.
            if (this._seriesDictionary.ContainsKey(item))
            {
                var old = this._seriesDictionary[item];
                old.XRangeChanged -= Sr_XRangeChanged;
                old.YRangeChanged -= Sr_YRangeChanged;
                this._seriesDictionary.Remove(item);
            }

            this._seriesDictionary.Add(item, sr);

            sr.XRangeChanged += Sr_XRangeChanged;
            sr.YRangeChanged += Sr_YRangeChanged;

            UpdateGlobalDataRange();

            OnPlotingYDataRangeChanged();
            OnPlotingXDataRangeChanged();

        }




        public GridLineControl GridLineControl
        {
            get { return (GridLineControl)GetValue(GridLineControlProperty); }
            set { SetValue(GridLineControlProperty, value); }
        }
        public static readonly DependencyProperty GridLineControlProperty =
            DependencyProperty.Register("GridLineControl", typeof(GridLineControl), typeof(SeriesChart), new PropertyMetadata(null, OnGridLineControlPropertyChanged));

        private static void OnGridLineControlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnGridLineControlChanged();
        }


        private void OnGridLineControlChanged()
        {

            if (this.PART_GridLineHolder == null)
            {
                return;
            }
            this.PART_GridLineHolder.Content = this.GridLineControl;

            this.GridLineControl?.OnAxisItemCoordinateChanged(Orientation.Vertical, this.YAxis?.GetAxisItemCoordinates());
            this.GridLineControl?.OnAxisItemCoordinateChanged(Orientation.Horizontal, this.XAxis?.GetAxisItemCoordinates());
        }


        #region Axises
        public AxisBase XAxis
        {
            get { return (AxisBase)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(AxisBase), typeof(SeriesChart), new PropertyMetadata(null, OnXAxisPropertyChanged));

        private static void OnXAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnXAxisPropertyChanged((AxisBase)e.OldValue, (AxisBase)e.NewValue);
        }
        private void OnXAxisPropertyChanged(AxisBase oldValue, AxisBase newValue)
        {
            if (this.PART_Root == null)
            {
                return;
            }

            if (oldValue != null)
            {

                this.PART_Root.Children.Remove(oldValue);
                oldValue.AxisPlacementChanged -= OnAxisPlacementChanged;
            }

            if (newValue != null)
            {
                if (this.PART_Root.Children.Contains(newValue))
                {
                    return;
                }
                if (newValue.Placement == AxisPlacement.None)
                {
                    newValue.Placement = AxisPlacement.Bottom;
                }

                (newValue.Parent as Panel)?.Children.Remove(newValue);
 
                this.PART_Root.Children.Add(newValue);
                newValue.Owner = this;
                newValue.Orientation = Orientation.Horizontal;
                newValue.AxisPlacementChanged += OnAxisPlacementChanged;
                OnAxisPlacementChanged(newValue);
            }
        }

        public AxisBase YAxis
        {
            get { return (AxisBase)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }
        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(AxisBase), typeof(SeriesChart), new PropertyMetadata(null, OnYAxisPropertyChanged));
        private static void OnYAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnYAxisPropertyChanged((AxisBase)e.OldValue, (AxisBase)e.NewValue);
        }
        private void OnYAxisPropertyChanged(AxisBase oldValue, AxisBase newValue)
        {
            if (this.PART_Root == null)
            {
                return;
            }

            if (oldValue != null)
            {
                this.PART_Root.Children.Remove(oldValue);
                oldValue.AxisPlacementChanged -= OnAxisPlacementChanged;
            }

            if (newValue != null)
            {
                if (this.PART_Root.Children.Contains(newValue))
                {
                    return;
                }

                if (newValue.Placement == AxisPlacement.None)
                {
                    newValue.Placement = AxisPlacement.Left;
                }

                (newValue.Parent as Panel)?.Children.Remove(newValue);

                this.PART_Root.Children.Add(newValue);
                newValue.Owner = this;
                newValue.Orientation = Orientation.Vertical;
                newValue.AxisPlacementChanged += OnAxisPlacementChanged;
                OnAxisPlacementChanged(newValue);
            }
        }

        private void OnAxisPlacementChanged(AxisBase obj)
        {
            switch ((Orientation)obj.Orientation)
            {
                case Orientation.Horizontal:
                    Grid.SetColumn(obj, 1);
                    switch (obj.Placement)
                    {
                        case AxisPlacement.Bottom:
                            Grid.SetRow(obj, 2);
                            break;
                        case AxisPlacement.Top:
                            Grid.SetRow(obj, 0);
                            break;
                        default:
                            throw new NotSupportedException($"XAxis does not support '{obj.Placement}' placement!");
                    }
                    break;
                case Orientation.Vertical:
                    Grid.SetRow(obj, 1);

                    switch (obj.Placement)
                    {
                        case AxisPlacement.Left:
                            Grid.SetColumn(obj, 0);
                            break;
                        case AxisPlacement.Right:
                            Grid.SetColumn(obj, 2);
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


        #region PlotAreaBackground
        public UIElement PlotAreaBackground
        {
            get { return (UIElement)GetValue(PlotAreaBackgroundProperty); }
            set { SetValue(PlotAreaBackgroundProperty, value); }
        }
        public static readonly DependencyProperty PlotAreaBackgroundProperty =
            DependencyProperty.Register("PlotAreaBackground", typeof(UIElement), typeof(SeriesChart), new PropertyMetadata(null, OnPlotAreaBackgroundPropertyChanged));

        private static void OnPlotAreaBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ((SeriesChart)d).OnPlotAreaBackgroundPropertyChanged((UIElement)e.OldValue, (UIElement)e.NewValue);
        }

        private void OnPlotAreaBackgroundPropertyChanged(UIElement oldValue, UIElement newValue)
        {
            if (this.PART_PlotingCanvas == null)
            {
                return;
            }

            if (this.PART_PlotingCanvas.Children.Contains(oldValue))
            {
                this.PART_PlotingCanvas.Children.Remove(oldValue);
            }

            if (newValue != null && !this.PART_PlotingCanvas.Children.Contains(newValue))
            {
                this.PART_PlotingCanvas.Children.Insert(0, newValue);
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



        private void PartPlotingCanvasMouseMove(object sender, MouseEventArgs e)
        {
            bool isHorizontalCrossHairVisible = this.HorizontalCrossHairVisibility == Visibility.Visible;
            bool isVerticalCrossHairVisible = this.VerticalCrossHairVisiblity == Visibility.Visible;

            if (!isHorizontalCrossHairVisible &&
                !isVerticalCrossHairVisible)
            {
                return;
            }

            var mousePoint = e.GetPosition(this.PART_PlotingCanvas);

            if (isHorizontalCrossHairVisible)
            {
                MoveCrossHairLine(this.PART_HorizontalCrossHair, mousePoint.X);
            }

            if (isVerticalCrossHairVisible)
            {
                MoveCrossHairLine(this.PART_VerticalCrossHair, mousePoint.Y);
            }


        }

        private void PartPlotingCanvasMouseLeave(object sender, MouseEventArgs e)
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
                crossHairLine.X1 = offset;
                crossHairLine.X2 = offset;
            }
            else
            {
                crossHairLine.Y1 = offset;
                crossHairLine.Y2 = offset;
            }

            if (crossHairLine.Visibility != Visibility.Visible)
            {
                crossHairLine.Visibility = Visibility.Visible;
            }


        }
        #endregion


        public void OnAxisItemsCoordinateChanged(Orientation orientation, IEnumerable<double> ticks)
        {
            this.GridLineControl?.OnAxisItemCoordinateChanged(orientation, ticks);
        }
    }
}
