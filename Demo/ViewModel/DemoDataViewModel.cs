using System;
using System.Collections.ObjectModel;
using System.Linq;
using MvvmChart.Common;
using MvvmCharting;

namespace Demo
{
    public class DemoDataViewModel : BindableBase
    {
        public ObservableCollection<SomePointList> ItemsSourceList { get; }

        private bool _showSeriesLine = true;
        public bool ShowSeriesLine
        {
            get { return _showSeriesLine; }
            set
            {

                if (SetProperty(ref _showSeriesLine, value))
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
            get { return _showSeriesPoints; }
            set
            {
                if (SetProperty(ref _showSeriesPoints, value))
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
        public DemoDataViewModel()
        {
            this.AvailableScatterTemplates = new ObservableCollection<string>()
            {
                "ScatterTemplate",
                "Scatter2Template"
            };

           

            this.ItemsSourceList = new ObservableCollection<SomePointList>();

            this.SelectedScatterTemplateType = this.AvailableScatterTemplates.First();

            var first = new SomePointList(0);
            for (int i = 0; i < 30; i++)
            {
                var v = i / 1.0;
                var y =  Math.Abs(v) < 1e-10 ? 1 : Math.Sin(v) / v;
                var pt = new SomePoint(v, y);
                first.DataList.Add(pt);
            }

            this.ItemsSourceList.Add(first);

            for (int i = 1; i < 3; i++)
            {
                var list = new SomePointList(i);
                double yOffset = i * 0.5;
                foreach (var item in first.DataList)
                {
                    list.DataList.Add(new SomePoint(item.t, item.Y + yOffset));
                }

                ItemsSourceList.Add(list);
            }






        }
    }
}