using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MvvmCharting.Common;
using MvvmCharting;

namespace Demo
{
    public class BigDataViewModel : BindableBase
    {
        public ObservableCollection<int> AvailableSizes { get; }
        public ObservableCollection<SomePointList> ItemsSourceList { get; }

        private bool _showSeriesLine = true;
        public bool ShowSeriesLine
        {
            get { return this._showSeriesLine; }
            set
            {

                if (SetProperty(ref this._showSeriesLine, value))
                {
                    foreach (var sr in this.ItemsSourceList)
                    {
                        sr.ShowLineSeries = value;
                    }
                }
            }
        }

        private bool _showSeriesPointsGlobal = false;
        public bool ShowSeriesPointsGlobal
        {
            get { return this._showSeriesPointsGlobal; }
            set
            {
                if (SetProperty(ref this._showSeriesPointsGlobal, value))
                {
                    UpdateScatterVisible();
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
                OnSelectedScatterTemplateTypeUpdated();
            }
        }

        private void OnSelectedScatterTemplateTypeUpdated( )
        {
            foreach (var list in this.ItemsSourceList)
            {
                list.SelectedScatterTemplateType = this.SelectedScatterTemplateType;
            }
        }

        private int _selectedDataSize = 1000;
        public int SelectedDataSize
        {
            get { return _selectedDataSize; }
            set
            {
                if (SetProperty(ref _selectedDataSize, value))
                {
                    RefreshData();
                }

            }
        }

        private void UpdateScatterVisible()
        {
            if (this.ItemsSourceList != null)
            {
                foreach (var sr in this.ItemsSourceList)
                {
                    sr.ShowScatterSeries = this.ShowSeriesPointsGlobal;
                }
            }

        }


        private void RefreshData()
        {
            this.ItemsSourceList.Clear();

            var list = new SomePointList(0);
            for (int i = 0; i < this.SelectedDataSize; i++)
            {
                var v = i / 100.0;
                var y = Math.Abs(v) < 1e-10 ? 1 : Math.Sin(v) / v;
                var pt = new SomePoint(v, y);
                list.DataList.Add(pt);
            }

            this.ItemsSourceList.Add(list);
            OnSelectedScatterTemplateTypeUpdated();
            UpdateScatterVisible();

        }

        public BigDataViewModel()
        {
            this.AvailableScatterTemplates = new ObservableCollection<string>()
            {
                "ScatterTemplate",
                "Scatter2Template"
            };

            this.ItemsSourceList = new ObservableCollection<SomePointList>();

            this.SelectedScatterTemplateType = this.AvailableScatterTemplates.First();

            this.AvailableSizes = new ObservableCollection<int>()
            {
                100, 1000, 5000, 10000, 15000, 20000, 90000
            };


            RefreshData();


        }
    }
}