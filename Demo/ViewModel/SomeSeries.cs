using System.Collections.ObjectModel;
using MvvmCharting;

namespace Demo
{
    public class SomeSeries: BindableBase
    {
        public int Index { get;  }

        public ObservableCollection<SomePoint> ItemsSource { get; }


        private bool _showSeriesLine = true;
        public bool ShowSeriesLine
        {
            get { return _showSeriesLine; }
            set { SetProperty(ref _showSeriesLine, value); }
        }

        private bool _showSeriesPoints = true;
        public bool ShowSeriesPoints
        {
            get { return _showSeriesPoints; }
            set { SetProperty(ref _showSeriesPoints, value); }
        }

        public SomeSeries(int index)
        {
            this.Index = index;
            this.ItemsSource = new ObservableCollection<SomePoint>();
          
        }
    }
}