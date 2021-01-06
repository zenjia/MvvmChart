using System.Collections.ObjectModel;
using MvvmCharting.Axis;
using MvvmCharting.Common;
using MvvmCharting.Series;
using MvvmCharting.WpfFX;
using MvvmCharting.WpfFX.Series;

namespace Demo
{
    public class ChartSettingTestViewModel : BindableBase
    {
        public ChartSettingTestViewModel()
        {
            this.XAxisPlacements = new ObservableCollection<AxisPlacement>();
            this.XAxisPlacements.Add(AxisPlacement.Bottom);
            this.XAxisPlacements.Add(AxisPlacement.Top);

            this.YAxisPlacements = new ObservableCollection<AxisPlacement>();
            this.YAxisPlacements.Add(AxisPlacement.Left);
            this.YAxisPlacements.Add(AxisPlacement.Right);

            this.SeriesModeList = new ObservableCollection<StackMode>()
            {
                StackMode.NotStacked,
                StackMode.Stacked,
                StackMode.Stacked100
            };

            this.SeriesGeometryBuilders = new ObservableCollection<string>()
            {
                "PolyLine","StepLine","Spline"
            };

        }

        public ObservableCollection<string> SeriesGeometryBuilders { get; }

        public ObservableCollection<StackMode> SeriesModeList { get; }

        public ObservableCollection<AxisPlacement> XAxisPlacements { get; }
        public ObservableCollection<AxisPlacement> YAxisPlacements { get; }

        private StackMode _selectedStackMode = StackMode.NotStacked;
        public StackMode SelectedStackMode
        {
            get { return this._selectedStackMode; }
            set { SetProperty(ref this._selectedStackMode, value); }
        }

        private string _selectedSeriesBuilder = "PolyLine";
        public string SelectedSeriesBuilder
        {
            get { return this._selectedSeriesBuilder; }
            set { SetProperty(ref this._selectedSeriesBuilder, value); }
        }



        private AxisPlacement _selectedXPlacement = AxisPlacement.Bottom;
        public AxisPlacement SelectedXPlacement
        {
            get { return this._selectedXPlacement; }
            set { SetProperty(ref this._selectedXPlacement, value); }
        }

        private AxisPlacement _selectedYPlacement = AxisPlacement.Left;
        public AxisPlacement SelectedYPlacement
        {
            get { return this._selectedYPlacement; }
            set { SetProperty(ref this._selectedYPlacement, value); }
        }

        private bool _showBackgroundImage;
        public bool ShowBackgroundImage
        {
            get { return this._showBackgroundImage; }
            set { SetProperty(ref this._showBackgroundImage, value); }
        }

        private bool _showGridLine = true;
        public bool ShowGridLine
        {
            get { return this._showGridLine; }
            set { SetProperty(ref this._showGridLine, value); }
        }

        private bool _showLegend = true;
        public bool ShowLegend
        {
            get { return this._showLegend; }
            set { SetProperty(ref this._showLegend, value); }
        }

    }
}