using System;
using System.Collections.ObjectModel;
using System.Linq;
using MvvmCharting;

namespace Demo
{
    public class DemoDataViewModel : BindableBase
    {
        private readonly Random _random = new Random();
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

        public DemoDataViewModel()
        {
            this.ItemsSourceList = new ObservableCollection<SomePointList>();

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