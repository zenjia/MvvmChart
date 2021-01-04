using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MvvmCharting.Common;
using MvvmCharting;

namespace Demo
{
    public class DemoDataViewModel : BindableBase
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
                    var chartValuePaddingViewModel = (ChartValuePaddingViewModel)Application.Current.Resources["ChartValuePaddingViewModel"];


                    if (chartValuePaddingViewModel.XMinValuePadding < 0.5)
                    {
                        chartValuePaddingViewModel.XMinValuePadding = 0.5;
                    }

                    if (chartValuePaddingViewModel.XMaxValuePadding < 0.5)
                    {
                        chartValuePaddingViewModel.XMaxValuePadding = 0.5;
                    }

                    foreach (var sr in this.ItemsSourceList)
                    {
                        sr.ShowBarSeries = value;
                    }
                }

            }
        }


        public ObservableCollection<string> AvailableScatterTemplates { get; }


        private string _selectedScatterTemplateType;
        public string SelectedScatterTemplateType
        {
            get { return this._selectedScatterTemplateType; }
            set
            {
                SetProperty(ref this._selectedScatterTemplateType, value);
                foreach (var list in this.ItemsSourceList)
                {
                    list.SelectedScatterTemplateType = value;
                }
            }
        }

        private bool _isModelChanging;
        public bool IsModelChanging
        {
            get { return _isModelChanging; }
            set { SetProperty(ref _isModelChanging, value); }
        }


        private int _max = 30;
        private int _min = 0;

        private SomePoint GetPoint(int i, double yOffset)
        {
            var v = i / 1.0;
            var y = Math.Abs(v) < 1e-10 ? 1 : Math.Sin(v) / v;

            var pt = new SomePoint(v, y + 0.25 + yOffset);

            return pt;
        }



        private void InitiateData()
        {
            for (int i = 0; i < 3; i++)
            {
                AddList(i);
            }

        }

        public DelegateCommand AddListCommand { get; private set; }
        public DelegateCommand RemoveListCommand { get; private set; }
        public DelegateCommand AddItemCommand { get; private set; }
        public DelegateCommand RemoveItemCommand { get; private set; }

        private void AddList(int index)
        {
            var list = new SomePointList(index);

            for (int j = this._min; j <= this._max; j++)
            {
                var pt = GetPoint(j, index * 0.5);
                list.DataList.Add(pt);
            }

            this.ItemsSourceList.Insert(0, list);
        }

        public void AddList()
        {
            int index = this.ItemsSourceList.Any() ? this.ItemsSourceList.Max(sr => sr.Index) + 1 : 0;

            AddList(index);
        }

        public void RemoveList()
        {
            if (!this.ItemsSourceList.Any())
            {
                return;
            }
            this.ItemsSourceList.RemoveAt(0);
        }

        public void AddItem()
        {
            IsModelChanging = true;
            try
            {
                this._max++;
                foreach (var list in this.ItemsSourceList)
                {
                    var pt = GetPoint(this._max, list.Index * 0.5);
                    list.DataList.Add(pt);
                }
            }
            finally
            {
                IsModelChanging = false;
            }


        }

        public void RemoveItem()
        {
            IsModelChanging = true;
            try
            {
                this._min++;

                for (int i = 0; i < this.ItemsSourceList.Count; i++)
                {
                    var list = this.ItemsSourceList[i];
                    if (list.DataList.Count > 3)
                    {
                        list.DataList.RemoveAt(0);
                    }

                }
            }
            finally
            {
                IsModelChanging = false;
            }

        }


        public DemoDataViewModel()
        {

            AddListCommand = new DelegateCommand((o) => AddList());
            RemoveListCommand = new DelegateCommand((o) => RemoveList());
            AddItemCommand = new DelegateCommand((o) => AddItem());
            RemoveItemCommand = new DelegateCommand((o) => RemoveItem());

            this.AvailableScatterTemplates = new ObservableCollection<string>()
            {
                "ScatterTemplate",
                "Scatter2Template"
            };



            this.ItemsSourceList = new ObservableCollection<SomePointList>();

            this.SelectedScatterTemplateType = this.AvailableScatterTemplates.First();


            InitiateData();





        }
    }
}