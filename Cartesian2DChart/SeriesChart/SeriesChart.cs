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
    public interface IAxisOwner
    {
        Thickness Padding { get; }
        event Action<Thickness> PaddingChanged;

        Thickness BorderThickness { get; }
        event Action<Thickness> BorderThicknessChanged;

        Thickness Margin { get; }
        event Action<Thickness> MarginChanged;
    }

    public interface IXAxisOwner : IAxisOwner
    {
        Range PlotAreaXDataRange { get; }

        event Action<Range> PlotAreaXRangeChanged;

        void OnXAxisTickChanged(IEnumerable<double> tickPositions);
    }

    public interface IYAxisOwner : IAxisOwner
    {
        Range PlotAreaYDataRange { get; }
        event Action<Range> PlotAreaYRangeChanged;
        void OnYAxisTickChanged(IEnumerable<double> tickPositions);
    }


    /// <summary>
    /// A Cartesian 2D Chart, which can displays a list of series(with item points).
    /// This is the host for almost everything: series plot area,
    /// x axis & y axis, grid lines, cross hair...
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_PlotAreaRoot", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_HorizontalGridLineItemsControl", Type = typeof(SlimItemsControl))]
    [TemplatePart(Name = "PART_VerticalGridLineItemsControl", Type = typeof(SlimItemsControl))]
    [TemplatePart(Name = "PART_SeriesItemsControl", Type = typeof(SlimItemsControl))]
    [TemplatePart(Name = "PART_HorizontalCrossHair", Type = typeof(Line))]
    [TemplatePart(Name = "PART_VerticalCrossHair", Type = typeof(Line))]
    public class SeriesChart : Control, IXAxisOwner, IYAxisOwner
    {
        static SeriesChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SeriesChart), new FrameworkPropertyMetadata(typeof(SeriesChart)));
        }

        private static readonly string sPART_Root = "PART_Root";
        private static readonly string sPART_PlotAreaRoot = "PART_PlotAreaRoot";
        private static readonly string sPART_HorizontalGridLineItemsControl = "PART_HorizontalGridLineItemsControl";
        private static readonly string sPART_VerticalGridLineItemsControl = "PART_VerticalGridLineItemsControl";
        private static readonly string sPART_SeriesItemsControl = "PART_SeriesItemsControl";
        private static readonly string sPART_HorizontalCrossHair = "PART_HorizontalCrossHair";
        private static readonly string sPART_VerticalCrossHair = "PART_VerticalCrossHair";

        public event Action<Thickness> PaddingChanged;
        public event Action<Thickness> MarginChanged;
        public event Action<Thickness> BorderThicknessChanged;
        public event Action<Range> PlotAreaXRangeChanged;
        public event Action<Range> PlotAreaYRangeChanged;

        private Grid PART_Root;
        private Grid PART_PlotAreaRoot;
        private SlimItemsControl PART_SeriesItemsControl;
        private SlimItemsControl PART_HorizontalGridLineItemsControl;
        private SlimItemsControl PART_VerticalGridLineItemsControl;
        private Line PART_HorizontalCrossHair;
        private Line PART_VerticalCrossHair;

        private Dictionary<object, SeriesBase> _seriesDictionary = new Dictionary<object, SeriesBase>();
        public SeriesChart()
        {

        }

        #region overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.PART_HorizontalGridLineItemsControl != null)
            {
                this.PART_HorizontalGridLineItemsControl.ItemTemplateContentLoaded -= HorizontalGridLineItemTemplateApplied;
            }

            if (this.PART_VerticalGridLineItemsControl != null)
            {
                this.PART_VerticalGridLineItemsControl.ItemTemplateContentLoaded += VerticalGridLineItemTemplateApplied;
            }

            if (this.PART_SeriesItemsControl != null)
            {
                this.PART_SeriesItemsControl.ItemTemplateContentLoaded -= SeriesItemTemplateApplied;
            }

            this.PART_SeriesItemsControl = (SlimItemsControl)GetTemplateChild(sPART_SeriesItemsControl);
            this.PART_SeriesItemsControl.ItemTemplateContentLoaded += SeriesItemTemplateApplied;

            this.PART_HorizontalGridLineItemsControl = (SlimItemsControl)GetTemplateChild(sPART_HorizontalGridLineItemsControl);
            this.PART_VerticalGridLineItemsControl = (SlimItemsControl)GetTemplateChild(sPART_VerticalGridLineItemsControl);
            this.PART_HorizontalGridLineItemsControl.ItemsSource = this.HorizontalGridLineOffsets;
            this.PART_VerticalGridLineItemsControl.ItemsSource = this.VerticalGridLineOffsets;

            this.PART_HorizontalGridLineItemsControl.ItemTemplateContentLoaded += HorizontalGridLineItemTemplateApplied;
            this.PART_VerticalGridLineItemsControl.ItemTemplateContentLoaded += VerticalGridLineItemTemplateApplied;

            this.PART_Root = (Grid)GetTemplateChild(sPART_Root);
            this.PART_PlotAreaRoot = (Grid)GetTemplateChild(sPART_PlotAreaRoot);
            OnXAxisPropertyChanged(null, this.XAxis);
            OnYAxisPropertyChanged(null, this.YAxis);
            OnPlotAreaBackgroundPropertyChanged(null, this.PlotAreaBackground);

            this.PART_HorizontalCrossHair = (Line)GetTemplateChild(sPART_HorizontalCrossHair);
            this.PART_VerticalCrossHair = (Line)GetTemplateChild(sPART_VerticalCrossHair);

            this.PART_PlotAreaRoot.MouseMove += PART_PlotAreaRoot_MouseMove;
            this.PART_PlotAreaRoot.MouseLeave += PART_PlotAreaRoot_MouseLeave;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == PaddingProperty)
            {
                this.PaddingChanged?.Invoke(this.Padding);
            }
            else if (e.Property == MarginProperty)
            {
                this.MarginChanged?.Invoke(this.Margin);
            }
            else if (e.Property == BorderThicknessProperty)
            {
                this.BorderThicknessChanged?.Invoke(this.BorderThickness);
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


        private Range _yDataRange = Range.Empty;

        public Range YDataRange
        {
            get { return this._yDataRange; }
            set
            {
                if (this._yDataRange != value)
                {
                    this._yDataRange = value;

                    UpdatePlotAreaYDataRange();
                }
            }
        }

        private Range _xDataRange = Range.Empty;

        public Range XDataRange
        {
            get { return this._xDataRange; }
            set
            {
                if (this._xDataRange != value)
                {
                    this._xDataRange = value;

                    UpdatePlotAreaXDataRange();
                }
            }
        }


        private void UpdateSeriesChartXRange()
        {
            foreach (var sr in this._seriesDictionary.Values)
            {
                sr.PlotAreaXDataRange = this.PlotAreaXDataRange;
            }
        }

        private void UpdateSeriesChartYRange()
        {
            foreach (var sr in this._seriesDictionary.Values)
            {
                sr.PlotAreaYDataRange = this.PlotAreaYDataRange;
            }
        }

        private void UpdateRange()
        {
            if (this._seriesDictionary.Count == 0)
            {
                this.XDataRange = Range.Empty;
                this.YDataRange = Range.Empty;
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
                this.XDataRange = new Range(minX, maxX);
            }

            if (!isYDataRangeEmplty)
            {
                this.YDataRange = new Range(minY, maxY);
            }


        }

        private Range _plotAreaXDataRange;
        public Range PlotAreaXDataRange
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
                    this.PlotAreaXRangeChanged?.Invoke(value);

                    UpdateSeriesChartXRange();
                }
            }
        }

        private Range _plotAreaYDataRange;
        public Range PlotAreaYDataRange
        {
            get { return this._plotAreaYDataRange; }
            set
            {
                if (this._plotAreaYDataRange != value)
                {
                    this._plotAreaYDataRange = value;
                    this.PlotAreaYRangeChanged?.Invoke(value);
                    UpdateSeriesChartYRange();
                }
            }
        }

        private void UpdatePlotAreaXDataRange()
        {
            double min = !this.XMinimum.IsNaN() ? this.XMinimum : this.XDataRange.Min;
            double max = !this.XMaximum.IsNaN() ? this.XMaximum : this.XDataRange.Max;

            if (min.IsNaN() && max.IsNaN())
            {
                this.PlotAreaXDataRange = Range.Empty;
                return;
            }

            if (this.PlotAreaXDataRange.IsEmpty ||
                !this.PlotAreaXDataRange.Min.NearlyEqual(min) ||
                !this.PlotAreaXDataRange.Max.NearlyEqual(max))
            {

                this.PlotAreaXDataRange = new Range(min, max);


            }
        }

        private void UpdatePlotAreaYDataRange()
        {
            double min = !this.YMinimum.IsNaN() ? this.YMinimum : this.YDataRange.Min;
            double max = !this.YMaximum.IsNaN() ? this.YMaximum : this.YDataRange.Max;

            if (min.IsNaN() && max.IsNaN())
            {
                this.PlotAreaYDataRange = Range.Empty;
                return;
            }

            if (this.PlotAreaYDataRange.IsEmpty ||
                this.PlotAreaYDataRange.Min != min ||
                this.PlotAreaYDataRange.Max != max)
            {

                this.PlotAreaYDataRange = new Range(min, max);
            }
        }


        private void Sr_XRangeChanged(Range obj)
        {
            UpdateRange();
        }

        private void Sr_YRangeChanged(Range obj)
        {
            UpdateRange();
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
                throw new Cartesian2DChartException("The root element in the SeriesDataTemplate should be of type: SeriesBase!");
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

            UpdateRange();

            UpdateSeriesChartYRange();
            UpdateSeriesChartXRange();

        }


        #region GridLine

        private double[] _horizontalTickOffsets;
        private double[] _verticalTickOffsets;

        private void SetGridLineBindings(DependencyObject rootChild, Orientation orientation)
        {
            if (!(rootChild is Line))
            {
                throw new Cartesian2DChartException($"The {orientation} grid line should be of type: Line!");
            }

            Line line = (Line)rootChild;


            Binding b = new Binding(orientation == Orientation.Horizontal ? nameof(this.PART_PlotAreaRoot.ActualWidth) : nameof(this.PART_PlotAreaRoot.ActualHeight));
            b.Source = this.PART_PlotAreaRoot;
            line.SetBinding(orientation == Orientation.Horizontal ? Line.X2Property : Line.Y2Property, b);

            b = new Binding();
            line.SetBinding(orientation == Orientation.Horizontal ? Line.Y1Property : Line.X1Property, b);

            b = new Binding();
            line.SetBinding(orientation == Orientation.Horizontal ? Line.Y2Property : Line.X2Property, b);

            b = new Binding(orientation == Orientation.Horizontal ? nameof(this.HorizontalGridLineStyle) : nameof(this.VerticalGridLineStyle));
            b.Source = this;
            line.SetBinding(StyleProperty, b);
        }

        private void VerticalGridLineItemTemplateApplied(object arg1, DependencyObject rootChild)
        {
            SetGridLineBindings(rootChild, Orientation.Vertical);
        }

        private void HorizontalGridLineItemTemplateApplied(object arg1, DependencyObject rootChild)
        {
            SetGridLineBindings(rootChild, Orientation.Horizontal);
        }




        public Visibility HorizontalGridLineVisiblility
        {
            get { return (Visibility)GetValue(HorizontalGridLineVisiblilityProperty); }
            set { SetValue(HorizontalGridLineVisiblilityProperty, value); }
        }
        public static readonly DependencyProperty HorizontalGridLineVisiblilityProperty =
            DependencyProperty.Register("HorizontalGridLineVisiblility", typeof(Visibility), typeof(SeriesChart), new PropertyMetadata(Visibility.Visible));



        public Visibility VerticalGridLineVisibility
        {
            get { return (Visibility)GetValue(VerticalGridLineVisibilityProperty); }
            set { SetValue(VerticalGridLineVisibilityProperty, value); }
        }
        public static readonly DependencyProperty VerticalGridLineVisibilityProperty =
            DependencyProperty.Register("VerticalGridLineVisibility", typeof(Visibility), typeof(SeriesChart), new PropertyMetadata(Visibility.Visible));




        public Style HorizontalGridLineStyle
        {
            get { return (Style)GetValue(HorizontalGridLineStyleProperty); }
            set { SetValue(HorizontalGridLineStyleProperty, value); }
        }
        public static readonly DependencyProperty HorizontalGridLineStyleProperty =
            DependencyProperty.Register("HorizontalGridLineStyle", typeof(Style), typeof(SeriesChart), new PropertyMetadata(null));


        public Style VerticalGridLineStyle
        {
            get { return (Style)GetValue(VerticalGridLineStyleProperty); }
            set { SetValue(VerticalGridLineStyleProperty, value); }
        }
        public static readonly DependencyProperty VerticalGridLineStyleProperty =
            DependencyProperty.Register("VerticalGridLineStyle", typeof(Style), typeof(SeriesChart), new PropertyMetadata(null));





        public void OnXAxisTickChanged(IEnumerable<double> tickPositions)
        {

            this._verticalTickOffsets = tickPositions.ToArray();
            UpdateVerticalGridLines();

        }

        public void OnYAxisTickChanged(IEnumerable<double> tickPositions)
        {


            this._horizontalTickOffsets = tickPositions.ToArray();
            UpdateHorizontalGridLines();
        }



        public ObservableCollection<double> HorizontalGridLineOffsets { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> VerticalGridLineOffsets { get; } = new ObservableCollection<double>();


        private void DoUpdateGridLines(ObservableCollection<double> target, double[] source)
        {
            var newCt = source.Length;
            var oldCt = target.Count;
            if (oldCt > newCt)
            {
                target.RemoveRange(newCt, oldCt - newCt);
            }
            else
            {
                for (int i = 0; i < source.Length; i++)
                {
                    var newValue = source[i];
                    if (i < oldCt)
                    {
                        target[i] = newValue;
                    }
                    else
                    {
                        target.Add(newValue);
                    }
                }
            }

        }

        private void UpdateHorizontalGridLines()
        {



            DoUpdateGridLines(this.HorizontalGridLineOffsets, this._horizontalTickOffsets);
        }

        private void UpdateVerticalGridLines()
        {
            DoUpdateGridLines(this.VerticalGridLineOffsets, this._verticalTickOffsets);
        }
        #endregion


        #region Axises
        public XAxis XAxis
        {
            get { return (XAxis)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(XAxis), typeof(SeriesChart), new PropertyMetadata(null, OnXAxisPropertyChanged));

        private static void OnXAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnXAxisPropertyChanged((XAxis)e.OldValue, (XAxis)e.NewValue);
        }
        private void OnXAxisPropertyChanged(XAxis oldValue, XAxis newValue)
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
                this.PART_Root.Children.Add(newValue);
                newValue.Owner = this;
                newValue.AxisPlacementChanged += OnAxisPlacementChanged;
                OnAxisPlacementChanged(newValue);
            }
        }

        public YAxis YAxis
        {
            get { return (YAxis)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }
        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(YAxis), typeof(SeriesChart), new PropertyMetadata(null, OnYAxisPropertyChanged));
        private static void OnYAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnYAxisPropertyChanged((YAxis)e.OldValue, (YAxis)e.NewValue);
        }
        private void OnYAxisPropertyChanged(YAxis oldValue, YAxis newValue)
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
                this.PART_Root.Children.Add(newValue);
                newValue.Owner = this;
                newValue.AxisPlacementChanged += OnAxisPlacementChanged;
                OnAxisPlacementChanged(newValue);
            }
        }

        private void OnAxisPlacementChanged(AxisBase obj)
        {
            if (obj is XAxis)
            {
                Grid.SetColumn(obj, 1);
                switch (((XAxis)obj).XAxisPlacement)
                {
                    case XAxisPlacement.Bottom:
                        Grid.SetRow(obj, 2);
                        break;
                    case XAxisPlacement.Top:
                        Grid.SetRow(obj, 0);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                Grid.SetRow(obj, 1);

                switch (((YAxis)obj).YAxisPlacement)
                {
                    case YAxisPlacement.Left:
                        Grid.SetColumn(obj, 0);
                        break;
                    case YAxisPlacement.Right:
                        Grid.SetColumn(obj, 2);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
            if (this.PART_PlotAreaRoot == null)
            {
                return;
            }

            if (this.PART_PlotAreaRoot.Children.Contains(oldValue))
            {
                this.PART_PlotAreaRoot.Children.Remove(oldValue);
            }

            if (newValue != null && !this.PART_PlotAreaRoot.Children.Contains(newValue))
            {
                this.PART_PlotAreaRoot.Children.Insert(0, newValue);
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



        private void PART_PlotAreaRoot_MouseMove(object sender, MouseEventArgs e)
        {
            bool isHorizontalCrossHairVisible = this.HorizontalCrossHairVisibility == Visibility.Visible;
            bool isVerticalCrossHairVisible = this.VerticalCrossHairVisiblity == Visibility.Visible;

            if (!isHorizontalCrossHairVisible &&
                !isVerticalCrossHairVisible)
            {
                return;
            }

            var mousePoint = e.GetPosition(this.PART_PlotAreaRoot);

            if (isHorizontalCrossHairVisible)
            {
                MoveCrossHairLine(this.PART_HorizontalCrossHair, mousePoint.X);
            }

            if (isVerticalCrossHairVisible)
            {
                MoveCrossHairLine(this.PART_VerticalCrossHair, mousePoint.Y);
            }


        }

        private void PART_PlotAreaRoot_MouseLeave(object sender, MouseEventArgs e)
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
    }
}
