using System;
using System.Collections.ObjectModel;
using MvvmCharting;

namespace Demo
{
    public class DemoDataViewModel : BindableBase
    {
        private readonly Random _random = new Random();
        public ObservableCollection<SomeSeries> ItemsSourceList { get; }

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
            this.ItemsSourceList = new ObservableCollection<SomeSeries>();

            for (int i = 0; i < 3; i++)
            {
                var ls = new SomeSeries(i);
                for (int j = 1; j <= 15; j++)
                {
                    var pt = new SomePoint(j, this._random.Next(i * 10, (i + 1) * 50));
                    ls.ItemsSource.Add(pt);
                }

                this.ItemsSourceList.Add(ls);
            }

        }
    }
}