using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using MvvmCharting.Axis;
using MvvmCharting.Common;
using MvvmCharting.Series;

namespace DemoViewModel
{
    public class DemoDataViewModelBase : BindableBase
    {
        public ChartValuePaddingViewModel ChartValuePaddingViewModel { get; }

        public DemoDataViewModelBase()
        {
            ChartValuePaddingViewModel = new ChartValuePaddingViewModel();
            this.XAxisPlacements = new ObservableCollection<AxisPlacement>();
            this.XAxisPlacements.Add(AxisPlacement.Bottom);
            this.XAxisPlacements.Add(AxisPlacement.Top);

            this.YAxisPlacements = new ObservableCollection<AxisPlacement>();
            this.YAxisPlacements.Add(AxisPlacement.Left);
            this.YAxisPlacements.Add(AxisPlacement.Right);

            this.SeriesModeList = new ObservableCollection<StackMode>()
            {
                StackMode.NotStacked,
                StackMode.Stacked,
                StackMode.Stacked100
            };

            this.SeriesGeometryBuilders = new ObservableCollection<string>()
            {
                "PolyLine","StepLine","Spline"
            };

        }


        public ObservableCollection<string> SeriesGeometryBuilders { get; }

        public ObservableCollection<StackMode> SeriesModeList { get; }

        public ObservableCollection<AxisPlacement> XAxisPlacements { get; }
        public ObservableCollection<AxisPlacement> YAxisPlacements { get; }

        private StackMode _selectedStackMode = StackMode.NotStacked;
        public StackMode SelectedStackMode
        {
            get { return this._selectedStackMode; }
            set { SetProperty(ref this._selectedStackMode, value); }
        }

        private string _selectedSeriesBuilder = "PolyLine";
        public string SelectedSeriesBuilder
        {
            get { return this._selectedSeriesBuilder; }
            set { SetProperty(ref this._selectedSeriesBuilder, value); }
        }



        private AxisPlacement _selectedXPlacement = AxisPlacement.Bottom;
        public AxisPlacement SelectedXPlacement
        {
            get { return this._selectedXPlacement; }
            set { SetProperty(ref this._selectedXPlacement, value); }
        }

        private AxisPlacement _selectedYPlacement = AxisPlacement.Left;
        public AxisPlacement SelectedYPlacement
        {
            get { return this._selectedYPlacement; }
            set { SetProperty(ref this._selectedYPlacement, value); }
        }

        private bool _showBackgroundImage;
        public bool ShowBackgroundImage
        {
            get { return this._showBackgroundImage; }
            set { SetProperty(ref this._showBackgroundImage, value); }
        }

        private bool _showGridLine = true;
        public bool ShowGridLine
        {
            get { return this._showGridLine; }
            set { SetProperty(ref this._showGridLine, value); }
        }

        private bool _showLegend = true;
        public bool ShowLegend
        {
            get { return this._showLegend; }
            set { SetProperty(ref this._showLegend, value); }
        }

        private double _yBaseValue;
        public double YBaseValue
        {
            get { return this._yBaseValue; }
            set { SetProperty(ref this._yBaseValue, value); }
        }


    }
    public class DemoDataViewModel : DemoDataViewModelBase
    {
        public ObservableCollection<SomePointList> ItemsSourceList { get; }

        private bool _showLineSeries = true;
        public bool ShowLineSeries
        {
            get { return this._showLineSeries; }
            set
            {

                if (SetProperty(ref this._showLineSeries, value))
                {
                    foreach (var sr in this.ItemsSourceList)
                    {
                        sr.ShowLineSeries = value;
                    }
                }
            }
        }

        private bool _showScatterSeries = true;
        public bool ShowScatterSeries
        {
            get { return this._showScatterSeries; }
            set
            {
                if (SetProperty(ref this._showScatterSeries, value))
                {
                    foreach (var sr in this.ItemsSourceList)
                    {
                        sr.ShowScatterSeries = value;
                    }
                }

            }
        }

        private bool _showAreaSeries = true;
        public bool ShowAreaSeries
        {
            get { return this._showAreaSeries; }
            set
            {
                if (SetProperty(ref this._showAreaSeries, value))
                {
                    foreach (var sr in this.ItemsSourceList)
                    {
                        sr.ShowAreaSeries = value;
                    }
                }

            }
        }

        private bool _showBarSeries = false;
        public bool ShowBarSeries
        {
            get { return this._showBarSeries; }
            set
            {
                if (SetProperty(ref this._showBarSeries, value))
                {

                  

                    if (ChartValuePaddingViewModel.XMinValuePadding < 0.5)
                    {
                        ChartValuePaddingViewModel.XMinValuePadding = 0.5;
                    }

                    if (ChartValuePaddingViewModel.XMaxValuePadding < 0.5)
                    {
                        ChartValuePaddingViewModel.XMaxValuePadding = 0.5;
                    }

                    foreach (var sr in this.ItemsSourceList)
                    {
                        sr.ShowBarSeries = value;
                    }
                }

            }
        }

        private bool _isModelChanging;
        public bool IsModelChanging
        {
            get { return _isModelChanging; }
            set { SetProperty(ref _isModelChanging, value); }
        }


        private int _max = 0;
        private int _min = 0;
        public int Max
        {
            get { return this._max; }
            set
            {
                IsModelChanging = true;
                try
                {
           
                    if (SetProperty(ref this._max, value))
                    {
                        UpdateData();
                    }
                    
                }
                finally
                {
                    IsModelChanging = false;
                }


            }
        }
        public int Min
        {
            get { return this._min; }
            set
            {
                IsModelChanging = true;
                try
                {
                    if (SetProperty(ref this._min, value))
                    {
                        UpdateData();
                    }

                }
                finally
                {
                    IsModelChanging = false;
                }

            }
        }

        private SomePoint GetPoint(int i, double yOffset)
        {
            var v = i / 1.0;
            var y = Math.Abs(v) < 1e-10 ? 1 : Math.Sin(v) / v;

            var pt = new SomePoint(v, y - 0.25 + yOffset);

            return pt;
        }


        private int _minInternal;
        private int _maxInternal;
        internal void UpdateData()
        {
            if (this.Max <= this.Min || this.SeriesCount == 0)
            {
                return;
            }

            if (this.Min > this._minInternal)
            {
                for (int j = this._minInternal; j < this.Min; j++)
                {
                    foreach (var list in this.ItemsSourceList)
                    {
                        list.DataList.RemoveAt(0);
                    }

                }
            }
            else if (this.Min < this._minInternal)
            {
                for (int j = this._minInternal - 1; j >= this.Min; j--)
                {
                    foreach (var list in this.ItemsSourceList)
                    {
                        var index = list.Index;
                        var pt = GetPoint(j, index * 0.5);
                        list.DataList.Insert(0, pt);
                    }

                }
            }

            if (this.Max < this._maxInternal)
            {
                for (int j = this.Max; j < this._maxInternal; j++)
                {
                    foreach (var list in this.ItemsSourceList)
                    {
                        list.DataList.RemoveAt(list.DataList.Count-1);
                    }

                }
            }
            else if (this.Max > this._maxInternal)
            {

            
                for (int j = this._maxInternal; j < this.Max; j++)
                {
                    foreach (var list in this.ItemsSourceList)
                    {
                        var index = list.Index;
                        var pt = GetPoint(j, index * 0.5);
                        list.DataList.Add(pt);
                    }

                }
            }




            this._minInternal = this.Min;
            this._maxInternal = this.Max;
        }


        private SomePointList GetList(int index)
        {
            var list = new SomePointList(index);

            for (int j = this.Min; j < this.Max; j++)
            {
                var pt = GetPoint(j, index * 0.5);
                list.DataList.Add(pt);
            }

            return list;
        }



        private int _seriesCount;
        public int SeriesCount
        {
            get { return _seriesCount; }
            set
            {
                if (SetProperty(ref _seriesCount, value))
                {
                    if (this.ItemsSourceList.Count > value)
                    {
                        for (int i = this.ItemsSourceList.Count - 1; i >= value; i--)
                        {
                            this.ItemsSourceList.RemoveAt(i);
                        }
                    }
                    else if (this.ItemsSourceList.Count < value)
                    {
                        this.IsModelChanging = true;
                        try
                        {
                            for (int i = this.ItemsSourceList.Count; i < value; i++)
                            {
                                int index = this.ItemsSourceList.Any() ? 
                                    this.ItemsSourceList.Max(sr => sr.Index) + 1 :
                                    0;
                                var list = GetList(index);
                                this.ItemsSourceList.Insert(0, list);
                            }
                        }
                        finally
                        {
                            this.IsModelChanging = false;
                        }
  
                    }

                    this._minInternal = this.Min;
                    this._maxInternal = this.Max;


                }
            }
        }



        //private int _itemCount;
        //public int ItemCount
        //{
        //    get { return _itemCount; }
        //    set
        //    {
        //        if (SetProperty(ref _itemCount, value))
        //        {
        //            this._max = value;
        //            UpdateData();
        //        }

        //    }
        //}

        public DemoDataViewModel()
        {
            this.ItemsSourceList = new ObservableCollection<SomePointList>();
            this.ItemsSourceList.CollectionChanged += ItemsSourceList_CollectionChanged;
        }

        private void ItemsSourceList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems!=null)
            {
                foreach (var somePointList in e.NewItems.OfType<SomePointList>())
                {
                    somePointList.ShowLineSeries = this.ShowLineSeries;
                    somePointList.ShowScatterSeries = this.ShowScatterSeries;
                    somePointList.ShowAreaSeries = this.ShowAreaSeries;
                    somePointList.ShowBarSeries = this.ShowBarSeries;
                    
                }
            }
        }
    }
}