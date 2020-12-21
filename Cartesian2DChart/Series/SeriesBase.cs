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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MvvmCharting
{
    [TemplatePart(Name = "PART_DataPointItemsControl", Type = typeof(ItemsControlEx))]
    public abstract class SeriesBase : Control
    {
        private static readonly string sPART_DataPointItemsControl = "PART_DataPointItemsControl";
        private ItemsControlEx PART_ItemsControl;

        public SeriesBase()
        {
            this.SizeChanged += SeriesBase_SizeChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.PART_ItemsControl != null)
            {
                this.PART_ItemsControl.ItemTemplateApplied -= PART_ItemsControl_ItemPointGenerated;
            }
            this.PART_ItemsControl = (ItemsControlEx)GetTemplateChild(sPART_DataPointItemsControl);
            this.PART_ItemsControl.ItemTemplateApplied += PART_ItemsControl_ItemPointGenerated;
        }

        private void SeriesBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePointsPosition();
        }

        public event Action<Range> XRangeChanged;
        public event Action<Range> YRangeChanged;
         

        public string IndependentValueProperty
        {
            get { return (string)GetValue(IndependentValuePropertyProperty); }
            set { SetValue(IndependentValuePropertyProperty, value); }
        }
        public static readonly DependencyProperty IndependentValuePropertyProperty =
            DependencyProperty.Register("IndependentValueProperty", typeof(string), typeof(SeriesBase), new PropertyMetadata(null));


        public string DependentValueProperty
        {
            get { return (string)GetValue(DependentValuePropertyProperty); }
            set { SetValue(DependentValuePropertyProperty, value); }
        }
        public static readonly DependencyProperty DependentValuePropertyProperty =
            DependencyProperty.Register("DependentValueProperty", typeof(string), typeof(SeriesBase), new PropertyMetadata(null));

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(SeriesBase), new PropertyMetadata(null, OnPointItemsSourcePropertyChanged));



        public bool IsSeriesPointsVisible
        {
            get { return (bool)GetValue(IsSeriesPointsVisibleProperty); }
            set { SetValue(IsSeriesPointsVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsSeriesPointsVisibleProperty =
            DependencyProperty.Register("IsSeriesPointsVisible", typeof(bool), typeof(SeriesBase), new PropertyMetadata(true));

        public bool IsSeriesLineVisible
        {
            get { return (bool)GetValue(IsSeriesLineVisibleProperty); }
            set { SetValue(IsSeriesLineVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsSeriesLineVisibleProperty =
            DependencyProperty.Register("IsSeriesLineVisible", typeof(bool), typeof(SeriesBase), new PropertyMetadata(true));

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public static readonly DependencyProperty StrokeProperty =
            Shape.StrokeProperty.AddOwner(typeof(SeriesBase));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner(typeof(SeriesBase), new PropertyMetadata(1.0));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            Shape.FillProperty.AddOwner(typeof(SeriesBase));
      


        private static void OnPointItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SeriesBase c = (SeriesBase)d;

            c.OnPointItemsSourcePropertyChanged((IList)e.OldValue, (IList)e.NewValue);



        }

        protected virtual void OnPointItemsSourcePropertyChanged(IList oldValue, IList newValue)
        {
          
            LoadItemPointViewModels(null, this.ItemsSource);

            if (oldValue is INotifyCollectionChanged)
            {
                
            }

            if (newValue is INotifyCollectionChanged)
            {
                //todo: weak event
                ((INotifyCollectionChanged)newValue).CollectionChanged += SeriesBase_CollectionChanged;
                
            }
        }

        private void SeriesBase_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.DataPointViewModels?.Clear();
            }

            LoadItemPointViewModels(e.OldItems, e.NewItems);
        }

        private Range _yDataRange;
        public Range YDataRange
        {
            get { return this._yDataRange; }
            private set
            {
                if (this._yDataRange != value)
                {
                    this._yDataRange = value;

                    this.YRangeChanged?.Invoke(this.YDataRange);

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

                    this.XRangeChanged?.Invoke(this.XDataRange);


                }
            }
        }

        private Range _plotAreaXDataRange;
        public Range PlotAreaXDataRange
        {
            get { return this._plotAreaXDataRange; }
            set
            {
                if (this._plotAreaXDataRange != value)
                {
                    this._plotAreaXDataRange = value;



                    UpdatePointsPosition();
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


                    UpdatePointsPosition();

                }
            }
        }

        public ObservableCollection<DataPointViewModel> DataPointViewModels
        {
            get { return (ObservableCollection<DataPointViewModel>)GetValue(DataPointViewModelsProperty); }
            set { SetValue(DataPointViewModelsProperty, value); }
        }
        public static readonly DependencyProperty DataPointViewModelsProperty =
            DependencyProperty.Register("DataPointViewModels", typeof(IList), typeof(SeriesBase), new PropertyMetadata(null));


        protected void LoadItemPointViewModels(IList oldValue, IList newValue)
        {
            if (oldValue != null)
            {
                foreach (var item in oldValue)
                {
                    this.DataPointViewModels.Remove(x => x.Item == item);
                }
            }

            if (newValue != null)
            {
                if (this.DataPointViewModels == null)
                {
                    this.DataPointViewModels = new ObservableCollection<DataPointViewModel>();
                }

                foreach (var item in newValue)
                {
                    var dpvm = new DataPointViewModel(item);
                    this.DataPointViewModels.Add(dpvm);
                }
            }

            UpdateSeriesRange();
            UpdatePointsPosition();
            UpdateShape();
        }

        private void UpdatePointsPosition()
        {
            if (this.PlotAreaXDataRange.IsEmpty ||
                this.PlotAreaYDataRange.IsEmpty ||
                this.RenderSize.IsInvalid() ||
                this.DataPointViewModels == null)
            {
                return;
            }
 
            var plotAreaSize = this.RenderSize;

            var xRange = this.PlotAreaXDataRange;
            var yRange = this.PlotAreaYDataRange;
            double xUnit = plotAreaSize.Width / xRange.Span;
            double yUnit = plotAreaSize.Height / yRange.Span;

            foreach (var dpvm in this.DataPointViewModels)
            {
                var pt = GetPointFromItem(dpvm.Item);
                dpvm.Position = new Point((pt.X - this.PlotAreaXDataRange.Min) * xUnit - 1, (pt.Y - this.PlotAreaYDataRange.Min) * yUnit - 1);
            }

  
            UpdateShape();
        }

       


  
        protected abstract void UpdateShape();
 






        internal Point GetPointFromItem(object item)
        {
            var t = item.GetType();
            var x = t.GetProperty(this.IndependentValueProperty).GetValue(item);
            var y = t.GetProperty(this.DependentValueProperty).GetValue(item);




            var pt = new Point(Convert.ToDouble(x), Convert.ToDouble(y));

            return pt;

        }

        private void UpdateSeriesRange()
        {
            double minX = double.MaxValue, minY = double.MaxValue,
                maxX = double.MinValue, maxY = double.MinValue;

            foreach (var item in this.DataPointViewModels)
            {
                var pt = GetPointFromItem(item.Item);

                var x = pt.X;
                var y = pt.Y;

                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);

                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);

            }

            this.XDataRange = new Range(minX, maxX);
            this.YDataRange = new Range(minY, maxY);
        }




        public DataTemplate PointDataTemplate
        {
            get { return (DataTemplate)GetValue(PointDataTemplateProperty); }
            set { SetValue(PointDataTemplateProperty, value); }
        }
        public static readonly DependencyProperty PointDataTemplateProperty =
            DependencyProperty.Register("PointDataTemplate", typeof(DataTemplate), typeof(SeriesBase), new PropertyMetadata(null));

        public DataTemplateSelector PointDataTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(PointDataTemplateSelectorProperty); }
            set { SetValue(PointDataTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty PointDataTemplateSelectorProperty =
            DependencyProperty.Register("PointDataTemplateSelector", typeof(DataTemplateSelector), typeof(SeriesBase), new PropertyMetadata(null));





        private void PART_ItemsControl_ItemPointGenerated(object arg1, DependencyObject root)
        {
            var itemPoint = VisualTreeHelper2.GetAllChildren(root).OfType<DataPoint>().SingleOrDefault();
            if (itemPoint == null)
            {
                throw new Cartesian2DChartException("The root element in the PointDataTemplate should be of DataPoint type!");
            }

            //if (itemPoint.GetBindingExpression(DataPoint.PositionProperty) != null)
            //{
            //    BindingOperations.ClearBinding();
            //}

            Binding b = new Binding();
            b.Path = new PropertyPath(nameof(DataPointViewModel.Position));
            itemPoint.SetBinding(DataPoint.PositionProperty, b);




        }
    }



}
