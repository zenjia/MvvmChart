using System;
using System.Collections.ObjectModel;
using MvvmChart.Common;
using MvvmCharting;

namespace Demo
{
    public class BigDataViewModel : BindableBase
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

        private bool _showSeriesPoints = false;
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

        public BigDataViewModel()
        {
            this.ItemsSourceList = new ObservableCollection<SomePointList>();

            var first = new SomePointList(0);
            for (int i = 0; i < 3000; i++)
            {
                var v = i / 100.0;
                var y = Math.Abs(v) < 1e-10 ? 1 : Math.Sin(v) / v;
                var pt = new SomePoint(v, y);
                first.DataList.Add(pt);
            }

            first.ShowSeriesPoints = false;

            this.ItemsSourceList.Add(first);

            for (int i = 1; i < 3; i++)
            {
                var list = new SomePointList(i);
                double yOffset = i * 0.5;
                foreach (var item in first.DataList)
                {
                    list.DataList.Add(new SomePoint(item.t, item.Y + yOffset));
                }
                list.ShowSeriesPoints = false;
                this.ItemsSourceList.Add(list);
            }






        }
    }
}