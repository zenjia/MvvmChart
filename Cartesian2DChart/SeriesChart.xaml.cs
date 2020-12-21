using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cartesian2DChart.Axis;

namespace Cartesian2DChart
{
    /// <summary>
    /// Interaction logic for SeriesChart.xaml
    /// </summary>
    public partial class SeriesChart : UserControl, IXAxisOwner, IYAxisOwner
    {
        public SeriesChart()
        {
            InitializeComponent();
            this.SizeChanged += this.SizeChangedHandler;

            this.PART_SeriesItemsControl.ItemTemplateRootGenerated += this.SeriesTemplateChildGenerated;
            this.PART_HorizontalGridLineItemsControl.ItemsSource = this.HorizontalGridLineOffsets;
            this.PART_VerticalGridLineItemsControl.ItemsSource = this.VerticalGridLineOffsets;

            this.PART_HorizontalGridLineItemsControl.ItemTemplateRootGenerated += this.PART_HorizontalGridLineItemsControl_ItemTemplateRootGenerated;
            this.PART_VerticalGridLineItemsControl.ItemTemplateRootGenerated += this.PART_VerticalGridLineItemsControl_ItemTemplateRootGenerated;


            OnYAxisPropertyChanged(null, this.YAxis);
            OnXAxisPropertyChanged(null, XAxis);
        }


        public double XMinimum
        {
            get { return (double)this.GetValue(XMinimumProperty); }
            set { this.SetValue(XMinimumProperty, value); }
        }
        public static readonly DependencyProperty XMinimumProperty =
            DependencyProperty.Register("XMinimum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN, OnXMinimumPropertyChanged));

        public double XMaximum
        {
            get { return (double)this.GetValue(XMaximumProperty); }
            set { this.SetValue(XMaximumProperty, value); }
        }
        public static readonly DependencyProperty XMaximumProperty =
            DependencyProperty.Register("XMaximum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN, OnXMinimumPropertyChanged));

        private static void OnXMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnXMinimumChanged();
        }

        private void OnXMinimumChanged()
        {
            this.UpdateViewportXDataRange();
        }

        public double YMinimum
        {
            get { return (double)this.GetValue(YMinimumProperty); }
            set { this.SetValue(YMinimumProperty, value); }
        }
        public static readonly DependencyProperty YMinimumProperty =
            DependencyProperty.Register("YMinimum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN, OnYMinimumPropertyChanged));

        public double YMaximum
        {
            get { return (double)this.GetValue(YMaximumProperty); }
            set { this.SetValue(YMaximumProperty, value); }
        }
        public static readonly DependencyProperty YMaximumProperty =
            DependencyProperty.Register("YMaximum", typeof(double), typeof(SeriesChart), new PropertyMetadata(double.NaN));


        private static void OnYMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnYMinimumChanged();
        }

        private void OnYMinimumChanged()
        {
            this.UpdateViewportYDataRange();
        }

        public DataTemplate SeriesDataTemplate
        {
            get { return (DataTemplate)this.GetValue(SeriesDataTemplateProperty); }
            set { this.SetValue(SeriesDataTemplateProperty, value); }
        }
        public static readonly DependencyProperty SeriesDataTemplateProperty =
            DependencyProperty.Register("SeriesDataTemplate", typeof(DataTemplate), typeof(SeriesChart), new PropertyMetadata(null));

        public DataTemplateSelector SeriesTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(SeriesTemplateSelectorProperty); }
            set { this.SetValue(SeriesTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty SeriesTemplateSelectorProperty =
            DependencyProperty.Register("SeriesTemplateSelector", typeof(DataTemplateSelector), typeof(SeriesChart), new PropertyMetadata(null));

        public IList SeriesItemsSource
        {
            get { return (IList)this.GetValue(SeriesItemsSourceProperty); }
            set { this.SetValue(SeriesItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty SeriesItemsSourceProperty =
            DependencyProperty.Register("SeriesItemsSource", typeof(IList), typeof(SeriesChart), new PropertyMetadata(null, OnSeriesItemsSourceChanged));

        private static void OnSeriesItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeriesChart)d).OnSeriesItemsSourceChanged((IList)e.OldValue, (IList)e.NewValue);
        }

        private void OnSeriesItemsSourceChanged(IList oldValue, IList newValue)
        {
            if (oldValue is INotifyCollectionChanged)
            {

            }

            if (newValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)newValue).CollectionChanged += this.SeriesItemsSource_CollectionChanged;
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


        private Dictionary<object, SeriesBase> _seriesDictionary = new Dictionary<object, SeriesBase>();

        private Range _yDataRange;
        public Range YDataRange
        {
            get { return this._yDataRange; }
            private set
            {
                if (this._yDataRange != value)
                {
                    this._yDataRange = value;

                    this.UpdateViewportYDataRange();
                }
            }
        }

        private Range _xDataRange;
        public Range XDataRange
        {
            get { return this._xDataRange; }
            private set
            {
                if (this._xDataRange != value)
                {
                    this._xDataRange = value;

                    this.UpdateViewportXDataRange();
                }
            }
        }




        private void UpdateSeriesChartXRange()
        {
            foreach (var sr in this._seriesDictionary.Values)
            {
                sr.ViewportXDataRange = this.ViewportXDataRange;
            }
        }

        private void UpdateSeriesChartYRange()
        {
            foreach (var sr in this._seriesDictionary.Values)
            {
                sr.ViewportYDataRange = this.ViewportYDataRange;
            }
        }

        private void UpdateRange()
        {
            double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;

            foreach (var sr in this._seriesDictionary.Values)
            {
                if (!sr.XDataRange.IsEmpty)
                {

                    minX = Math.Min(minX, sr.XDataRange.Min);
                    maxX = Math.Max(maxX, sr.XDataRange.Max);
                }

                if (!sr.YDataRange.IsEmpty)
                {
                    minY = Math.Min(minY, sr.YDataRange.Min);
                    maxY = Math.Max(maxY, sr.YDataRange.Max);
                }

            }

            this.XDataRange = new Range(minX, maxX);
            this.YDataRange = new Range(minY, maxY);
        }

        private Range _viewportXDataRange;
        public Range ViewportXDataRange
        {
            get { return this._viewportXDataRange; }
            set
            {
                if (this._viewportXDataRange != value)
                {
                    this._viewportXDataRange = value;
                    this.ActualXRangeChanged?.Invoke(value);
                    this.UpdateSeriesChartXRange();
                }
            }
        }

        private Range _viewportYDataRange;
        public Range ViewportYDataRange
        {
            get { return this._viewportYDataRange; }
            set
            {
                if (this._viewportYDataRange != value)
                {
                    this._viewportYDataRange = value;
                    this.ActualYRangeChanged?.Invoke(value);
                    this.UpdateSeriesChartYRange();
                }
            }
        }

        private void UpdateViewportXDataRange()
        {
            double min = !this.XMinimum.IsNaN() ? this.XMinimum : this.XDataRange.Min;
            double max = !this.XMaximum.IsNaN() ? this.XMaximum : this.XDataRange.Max;

            if (min.IsNaN() && max.IsNaN())
            {
                this.ViewportXDataRange = Range.Empty;
                return;
            }

            if (this.ViewportXDataRange.IsEmpty ||
                this.ViewportXDataRange.Min != min ||
                this.ViewportXDataRange.Max != max)
            {
                this.ViewportXDataRange = new Range(min, max);
            }
        }

        private void UpdateViewportYDataRange()
        {
            double min = !this.YMinimum.IsNaN() ? this.YMinimum : this.YDataRange.Min;
            double max = !this.YMaximum.IsNaN() ? this.YMaximum : this.YDataRange.Max;

            if (min.IsNaN() && max.IsNaN())
            {
                this.ViewportYDataRange = Range.Empty;
                return;
            }

            if (this.ViewportYDataRange.IsEmpty ||
                this.ViewportYDataRange.Min != min ||
                this.ViewportYDataRange.Max != max)
            {
                this.ViewportYDataRange = new Range(min, max);
            }
        }

        private void SeriesTemplateChildGenerated(object sender, DependencyObject root)
        {

            var sr = root as SeriesBase;
            if (sr == null)
            {
                throw new Cartesian2DChartException("The root element in the SeriesDataTemplate should be of type: SeriesBase!");
            }

            var item = sr.DataContext;

            if (this._seriesDictionary.ContainsKey(item))
            {
                var old = this._seriesDictionary[item];
                old.XRangeChanged -= this.Sr_XRangeChanged;
                old.YRangeChanged -= this.Sr_YRangeChanged;
                this._seriesDictionary.Remove(item);

                //throw new Cartesian2DChartException("The ItemTemplate of an ItemsControl should only be applied once!");
            }

            this._seriesDictionary.Add(item, sr);

            sr.XRangeChanged += this.Sr_XRangeChanged;
            sr.YRangeChanged += this.Sr_YRangeChanged;

            this.UpdateRange();

            this.UpdateSeriesChartYRange();
            this.UpdateSeriesChartXRange();

        }

        private void Sr_XRangeChanged(Range obj)
        {
            this.UpdateRange();
        }

        private void Sr_YRangeChanged(Range obj)
        {
            this.UpdateRange();
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            //this.PART_root.Clip = new RectangleGeometry(new Rect(0, -Double.MaxValue, e.NewSize.Width, Double.MaxValue));

            ChartSizeChanged?.Invoke(e.NewSize);


        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == Control.PaddingProperty)
            {
                PaddingChanged?.Invoke(this.Padding);
            }
        }


        public Size ChartSize => this.RenderSize;

        //private ItemsControlEx PART_SeriesItemsControl;








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
            }

            if (newValue != null)
            {
                this.PART_Root.Children.Add(newValue);
                Grid.SetColumn(newValue, 1);
                Grid.SetRow(newValue, 2);
                newValue.AxisOwner = this;
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
            }

            if (newValue != null)
            {
                this.PART_Root.Children.Add(newValue);
                Grid.SetColumn(newValue, 0);
                Grid.SetRow(newValue, 1);
                newValue.AxisOwner = this;
            }
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

            Binding b = new Binding(orientation == Orientation.Horizontal ? nameof(this.ActualWidth) : nameof(this.ActualHeight));
            b.Source = this;
            line.SetBinding(orientation == Orientation.Horizontal ? Line.X2Property : Line.Y2Property, b);

            b = new Binding();
            line.SetBinding(orientation == Orientation.Horizontal ? Line.Y1Property : Line.X1Property, b);

            b = new Binding();
            line.SetBinding(orientation == Orientation.Horizontal ? Line.Y2Property : Line.X2Property, b);

            b = new Binding(orientation == Orientation.Horizontal ? nameof(this.HorizontalGridLineStyle) : nameof(this.VerticalGridLineStyle));
            b.Source = this;
            line.SetBinding(Line.StyleProperty, b);
        }

        private void PART_VerticalGridLineItemsControl_ItemTemplateRootGenerated(object arg1, DependencyObject rootChild)
        {
            SetGridLineBindings(rootChild, Orientation.Vertical);
        }

        private void PART_HorizontalGridLineItemsControl_ItemTemplateRootGenerated(object arg1, DependencyObject rootChild)
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

        public event Action<Size> ChartSizeChanged;
        public event Action<Thickness> PaddingChanged;
        public event Action<Range> ActualXRangeChanged;
        public event Action<Range> ActualYRangeChanged;
    }
}
