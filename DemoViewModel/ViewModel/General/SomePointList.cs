using System.Collections.ObjectModel;
using MvvmCharting.Common;

namespace DemoViewModel
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

        private bool _showLineSeries = true;
        public bool ShowLineSeries
        {
            get { return this._showLineSeries; }
            set { SetProperty(ref this._showLineSeries, value); }
        }

        private bool _showAreaSeries = true;
        public bool ShowAreaSeries
        {
            get { return this._showAreaSeries; }
            set
            {
              
                SetProperty(ref this._showAreaSeries, value);
            }
        }

        private bool _showBarSeries = false;
        public bool ShowBarSeries
        {
            get { return this._showBarSeries; }
            set
            {

                SetProperty(ref this._showBarSeries, value);
            }
        }


        private bool _showScatterSeries = true;
        public bool ShowScatterSeries
        {
            get { return this._showScatterSeries; }
            set
            {
                SetProperty(ref this._showScatterSeries, value);
            }
        }

        public SomePointList(int index)
        {
            this.Index = index;
            this.DataList = new ObservableCollection<SomePoint>();

        }
    }
}