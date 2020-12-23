using System.Collections.ObjectModel;
using MvvmChart.Common;
using MvvmCharting;

namespace Demo
{
    public class TimeSeriesData : BindableBase
    {
        public int Index { get; }

        public ObservableCollection<TimeSeriesPoint> DataList { get; }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return this._isVisible; }
            set { SetProperty(ref this._isVisible, value); }
        }

        private bool _showSeriesLine = true;
        public bool ShowSeriesLine
        {
            get { return this._showSeriesLine; }
            set { SetProperty(ref this._showSeriesLine, value); }
        }

        private bool _showSeriesPoints = true;
        public bool ShowSeriesPoints
        {
            get { return this._showSeriesPoints; }
            set { SetProperty(ref this._showSeriesPoints, value); }
        }

        public TimeSeriesData(int index)
        {
            this.Index = index;
            this.DataList = new ObservableCollection<TimeSeriesPoint>();

        }
    }
}