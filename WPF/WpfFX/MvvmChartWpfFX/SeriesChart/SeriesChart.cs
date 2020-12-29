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
    public class SeriesChart : Control
    {
        private static readonly string sPART_SeriesItemsControl = "PART_SeriesItemsControl";
        static SeriesChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SeriesChart), new FrameworkPropertyMetadata(typeof(SeriesChart)));
        }

        private SlimItemsControl PART_SeriesItemsControl;

        private int SeriesCount => this.PART_SeriesItemsControl?.ItemCount ?? 0;

        private IEnumerable<ISeries> GetSeries()
        {
            if (this.PART_SeriesItemsControl == null)
            {
                return Enumerable.Empty<ISeries>();
            }

            return this.PART_SeriesItemsControl.GetAllElements().OfType<ISeries>();
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

                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemTemplateProperty,
                    new Binding(nameof(this.SeriesTemplate)) { Source = this });
                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemTemplateSelectorProperty,
                    new Binding(nameof(this.SeriesTemplateSelector)) { Source = this });
                this.PART_SeriesItemsControl.SetBinding(SlimItemsControl.ItemsSourceProperty,
                    new Binding(nameof(this.SeriesItemsSource)) { Source = this });
            }

 
        }

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


            sr.XRangeChanged += Sr_XValueRangeChanged;
            sr.YRangeChanged += Sr_YValueRangeChanged;
            //sr.PropertyChanged += Sr_PropertyChanged;


            sr.UpdateValueRange();


        }

        //private void Sr_PropertyChanged(object sender, string propertyName)
        //{
        //    var sr = (ISeries)sender;
        //    if (propertyName == nameof(sr.IsHighLighted))
        //    {
        //        this.Legend.OnItemHighlightChanged(sr.DataContext, sr.IsHighLighted);
        //    }

        //}

        #region SeriesDataTemplate & SeriesTemplateSelector
        public DataTemplate SeriesTemplate
        {
            get { return (DataTemplate)GetValue(SeriesTemplateProperty); }
            set { SetValue(SeriesTemplateProperty, value); }
        }
        public static readonly DependencyProperty SeriesTemplateProperty =
            DependencyProperty.Register("SeriesTemplate", typeof(DataTemplate), typeof(SeriesChart), new PropertyMetadata(null));

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
            DependencyProperty.Register("SeriesItemsSource", typeof(IList), typeof(SeriesChart), new PropertyMetadata(null));

 
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

        private void Sr_XValueRangeChanged(Range obj)
        {
            UpdateGlobalValueRange();
        }

        private void Sr_YValueRangeChanged(Range obj)
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
        public Range PlottingXValueRange
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
                        sr.PlottingXValueRange = this.PlottingXValueRange;
                    }
                }
            }
        }

        private Range _plottingYValueRange = Range.Empty;
        /// <summary>
        /// The final dependent value range(min & max) used to plot series chart
        /// </summary>
        public Range PlottingYValueRange
        {
            get { return this._plottingYValueRange; }
            set
            {
                if (this._plottingYValueRange != value)
                {
                    this._plottingYValueRange = value;

                    foreach (var sr in this.GetSeries())
                    {
                        sr.PlottingYValueRange = this.PlottingYValueRange;
                    }
                }
            }
        }

 


        #endregion


    }
}
