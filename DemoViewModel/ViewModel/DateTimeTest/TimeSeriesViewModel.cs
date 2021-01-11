using System;
using System.Collections.ObjectModel;
using MvvmCharting.Common;

namespace DemoViewModel
{
    public class TimeSeriesViewModel : BindableBase
    {
        public ObservableCollection<TimeSeriesData> ItemsSourceList { get; }

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

        public TimeSeriesViewModel()
        {
            this.ItemsSourceList = new ObservableCollection<TimeSeriesData>();

            var first = new TimeSeriesData(0);
            DateTimeOffset now = DateTimeOffset.Now;
            for (int i = 0; i < 30; i++)
            {

                var t = now.AddMonths(i);
                var v = i / 1.0;
                var y = Math.Abs(v) < 1e-10 ? 1 : Math.Sin(v) / v;
                var pt = new TimeSeriesPoint(t, y);
                first.DataList.Add(pt);
            }

            this.ItemsSourceList.Add(first);

            for (int i = 1; i < 3; i++)
            {
                var list = new TimeSeriesData(i);
                double yOffset = i * 0.5;
                foreach (var item in first.DataList)
                {
                    list.DataList.Add(new TimeSeriesPoint(item.t, item.Y + yOffset));
                }

                this.ItemsSourceList.Add(list);
            }






        }
    }
}