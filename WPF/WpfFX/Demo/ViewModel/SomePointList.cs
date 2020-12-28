using System.Collections.ObjectModel;
using System.Diagnostics;
using MvvmCharting.Common;
using MvvmCharting;

namespace Demo
{
    public class SomePointList: BindableBase
    {
        public override string ToString()
        {
            return $"This is series {Index}";
        }

        public int Index { get;  }

        private string _selectedScatterTemplateType;
        public string SelectedScatterTemplateType
        {
            get { return this._selectedScatterTemplateType; }
            set { SetProperty(ref this._selectedScatterTemplateType, value); }
        }


        public ObservableCollection<SomePoint> DataList { get; }

        private bool _isVisible=true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
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
            get { return _showSeriesLine; }
            set { SetProperty(ref _showSeriesLine, value); }
        }

        private bool _showSeriesPoints = true;
        public bool ShowSeriesPoints
        {
            get { return _showSeriesPoints; }
            set
            {
                SetProperty(ref _showSeriesPoints, value);
            }
        }

        public SomePointList(int index)
        {
            this.Index = index;
            this.DataList = new ObservableCollection<SomePoint>();

        }
    }
}