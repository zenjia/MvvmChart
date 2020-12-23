using System.Collections.ObjectModel;
using MvvmChart.Common;
using MvvmCharting;

namespace Demo
{
    public class SomePointList: BindableBase
    {
        public int Index { get;  }

        public ObservableCollection<SomePoint> DataList { get; }

        private bool _isVisible=true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }

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

        public SomePointList(int index)
        {
            this.Index = index;
            this.DataList = new ObservableCollection<SomePoint>();
          
        }
    }
}