using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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
using MvvmCharting.Axis;
using MvvmCharting.Common;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{

    /// <summary>
    /// This is used to plot one series or a collection of series.
    /// A series is composed of a curve(or a area) and a collection of Scatters
    /// </summary>
    [TemplatePart(Name = "PART_SeriesItemsControl", Type = typeof(SlimItemsControl))]
    public class SeriesControl : Control, ISeriesHost
    {
        private static readonly string sPART_SeriesItemsControl = "PART_SeriesItemsControl";
        static SeriesControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SeriesControl), new FrameworkPropertyMetadata(typeof(SeriesControl)));
        }

        private SlimItemsControl PART_SeriesItemsControl;

        private bool _isXAxisCategory;
        public bool IsXAxisCategory
        {
            get { return this._isXAxisCategory; }
            internal set
            {
                if (this._isXAxisCategory != value)
                {
                    this._isXAxisCategory = value;

                    if (value)
                    {
                        EnsureXValueUniformity();
                    }
                }
            }
        }

        private void EnsureXValueUniformity()
        {
            SeriesBase firstSr = null;
            foreach (var sr in this.GetSeries())
            {
                if (firstSr == null)
                {
                    firstSr = sr;
                    continue;
                }

                if (sr.ItemsSource.Count != firstSr.ItemsSource.Count)
                {
                    throw new MvvmChartException("If the XAxis is CategoryAxis, the ItemsSource of all series should have the same length!");
                }

                for (int i = 0; i < sr.ItemsSource.Count; i++)
                {
                    if (sr.GetXRawValueForItem(sr.ItemsSource[i]) == firstSr.GetXRawValueForItem(firstSr.ItemsSource[i]))
                    {
                        throw new MvvmChartException("If the XAxis is CategoryAxis, the ItemsSource of all series should have the same x value at each index!");
                    }
                }


            }
        }

        public bool IsSeriesCollectionChanging { get; set; }

        public int SeriesCount => this.PART_SeriesItemsControl?.ItemCount ?? 0;

        public IEnumerable<SeriesBase> GetSeries()
        {
            if (this.PART_SeriesItemsControl == null)
            {
                return Enumerable.Empty<SeriesBase>();
            }

            return this.PART_SeriesItemsControl.GetChildren().OfType<SeriesBase>();
        }

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
                //this.PART_SeriesItemsControl.ItemAdded += PART_SeriesItemsControl_ItemAdded;
                this.PART_SeriesItemsControl.ChildRemoved += PartSeriesChildrenControlChildRemoved;
                //this.PART_SeriesItemsControl.ItemReplaced += PART_SeriesItemsControl_ItemReplaced;
                this.PART_SeriesItemsControl.Reset += PART_SeriesItemsControl_Reset;

                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemTemplateProperty,
                    new Binding(nameof(this.SeriesTemplate)) { Source = this });
                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemTemplateSelectorProperty,
                    new Binding(nameof(this.SeriesTemplateSelector)) { Source = this });
                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemsSourceProperty,
                    new Binding(nameof(this.SeriesItemsSource)) { Source = this });
            }


        }

        private void PART_SeriesItemsControl_Reset(object obj)
        {
            UpdateGlobalValueRange();
        }

        private void PartSeriesChildrenControlChildRemoved(object arg1, FrameworkElement arg2)
        {
            UpdateGlobalValueRange();
        }

        private void SeriesItemTemplateApplied(object sender, DependencyObject root, int index)
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

            (sr as SeriesBase).Owner = this;

            sr.XRangeChanged += Sr_XValueRangeChanged;
            sr.YRangeChanged += Sr_YValueRangeChanged;

            sr.OnPlottingXValueRangeChanged(this.PlottingXValueRange);
            sr.OnPlottingYValueRangeChanged(this.PlottingYValueRange);

            sr.UpdateValueRange();


        }

        #region SeriesDataTemplate & SeriesTemplateSelector
        public DataTemplate SeriesTemplate
        {
            get { return (DataTemplate)GetValue(SeriesTemplateProperty); }
            set { SetValue(SeriesTemplateProperty, value); }
        }
        public static readonly DependencyProperty SeriesTemplateProperty =
            DependencyProperty.Register("SeriesTemplate", typeof(DataTemplate), typeof(SeriesControl), new PropertyMetadata(null));

        public DataTemplateSelector SeriesTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SeriesTemplateSelectorProperty); }
            set { SetValue(SeriesTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty SeriesTemplateSelectorProperty =
            DependencyProperty.Register("SeriesTemplateSelector", typeof(DataTemplateSelector), typeof(SeriesControl), new PropertyMetadata(null));
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
            DependencyProperty.Register("SeriesItemsSource", typeof(IList), typeof(SeriesControl));
        #endregion

        #region Global Data Range
        private Range _globalYValueRange = Range.Empty;
        /// <summary>
        /// The dependent value Range(min & max) of all series data
        /// </summary>
        public Range GlobalYValueRange
        {
            get { return this._globalYValueRange; }
            set
            {
                if (this._globalYValueRange != value)
                {
                    this._globalYValueRange = value;
                    this.GlobalYValueRangeChanged?.Invoke(value);
                }
            }
        }

        private Range _globalXValueRange = Range.Empty;
        /// <summary>
        /// The independent value Range(min & max) of all series data
        /// </summary>
        public Range GlobalXValueRange
        {
            get { return this._globalXValueRange; }
            set
            {
                if (this._globalXValueRange != value)
                {
                    this._globalXValueRange = value;


                    this.GlobalXValueRangeChanged?.Invoke(value);
                }
            }
        }

        public event Action<Range> GlobalXValueRangeChanged;
        public event Action<Range> GlobalYValueRangeChanged;

        private void Sr_XValueRangeChanged(ISeries sr, Range obj)
        {
            UpdateGlobalValueRange();
        }

        private void Sr_YValueRangeChanged(ISeries sr, Range obj)
        {
            UpdateGlobalValueRange();
        }

        private void UpdateGlobalValueRange()
        {
            if (this.SeriesCount == 0)
            {
                this.GlobalXValueRange = Range.Empty;
                this.GlobalYValueRange = Range.Empty;
                return;
            }

            double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;

            bool isXDataRangeEmplty = true;
            bool isYDataRangeEmplty = true;

            foreach (var sr in this.GetSeries())
            {

                if (!sr.XValueRange.IsEmpty)
                {
                    minX = Math.Min(minX, sr.XValueRange.Min);
                    maxX = Math.Max(maxX, sr.XValueRange.Max);

                    if (isXDataRangeEmplty)
                    {
                        isXDataRangeEmplty = false;
                    }

                }

                if (!sr.YValueRange.IsEmpty)
                {
                    minY = Math.Min(minY, sr.YValueRange.Min);
                    maxY = Math.Max(maxY, sr.YValueRange.Max);
                    if (isYDataRangeEmplty)
                    {
                        isYDataRangeEmplty = false;
                    }
                }

            }



            if (!isXDataRangeEmplty)
            {
                this.GlobalXValueRange = new Range(minX, maxX);
            }

            if (!isYDataRangeEmplty)
            {
                this.GlobalYValueRange = new Range(minY, maxY);
            }


        }
        #endregion

        #region Plotting Data value Range
        private Range _plottingXValueRange = Range.Empty;
        /// <summary>
        /// The final independent value range(min & max) used to plot series chart
        /// </summary>
        protected Range PlottingXValueRange
        {
            get
            {

                return this._plottingXValueRange;
            }
            set
            {
                if (this._plottingXValueRange != value)
                {
                    this._plottingXValueRange = value;

                    foreach (var sr in this.GetSeries())
                    {
                        sr.OnPlottingXValueRangeChanged(value);
                    }
                }
            }
        }

        private Range _plottingYValueRange = Range.Empty;
        /// <summary>
        /// The final dependent value range(min & max) used to plot series chart
        /// </summary>
        protected Range PlottingYValueRange
        {
            get { return this._plottingYValueRange; }
            set
            {
                if (this._plottingYValueRange != value)
                {
                    this._plottingYValueRange = value;

                    foreach (var sr in this.GetSeries())
                    {
                        sr.OnPlottingYValueRangeChanged(value);
                    }
                }
            }
        }

        internal virtual void SetPlottingValueRange(Orientation orientation, Range newValue)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    this.PlottingXValueRange = newValue;
                    break;
                case Orientation.Vertical:
                    this.PlottingYValueRange = newValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }
        #endregion

        internal void UpdateSeriesCoordinates()
        {
            foreach (var sr in this.GetSeries())
            {
                sr.UpdateValueRange();
                sr.RecalculateCoordinate();
            }
        }
    }
}
