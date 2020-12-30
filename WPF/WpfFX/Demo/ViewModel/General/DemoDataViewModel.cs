using System;
using System.Collections.ObjectModel;
using System.Linq;
using MvvmCharting.Common;
using MvvmCharting;

namespace Demo
{
    public class DemoDataViewModel : BindableBase
    {

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
                        sr.ShowSeriesLine = value;
                    }
                }
            }
        }

        private bool _showSeriesPoints = true;
        public bool ShowSeriesPoints
        {
            get { return this._showSeriesPoints; }
            set
            {
                if (SetProperty(ref this._showSeriesPoints, value))
                {
                    foreach (var sr in this.ItemsSourceList)
                    {
                        sr.ShowSeriesPoints = value;
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

        public void AddData()
        {
            this._max++;
            foreach (var list in this.ItemsSourceList)
            {
                var pt = GetPoint(this._max, list.Index * 0.5);
                list.DataList.Add(pt);
            }

        }

        public void RemoveData()
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


        public DemoDataViewModel()
        {
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