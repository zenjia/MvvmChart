using System.Collections.ObjectModel;
using MvvmCharting.Common;

namespace DemoViewModel
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

        private bool _isHighlighted;
        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set { SetProperty(ref _isHighlighted, value); }
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