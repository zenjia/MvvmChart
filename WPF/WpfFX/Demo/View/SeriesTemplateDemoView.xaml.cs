using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MvvmCharting.Axis;
using MvvmCharting.Common;

namespace Demo
{
    public class ChartSettingTestViewModel : BindableBase
    {
        public ChartSettingTestViewModel()
        {
            XAxisPlacements = new ObservableCollection<AxisPlacement>();
            XAxisPlacements.Add(AxisPlacement.Bottom);
            XAxisPlacements.Add(AxisPlacement.Top);

            this.YAxisPlacements = new ObservableCollection<AxisPlacement>();
            this.YAxisPlacements.Add(AxisPlacement.Left);
            this.YAxisPlacements.Add(AxisPlacement.Right);
        }

        public ObservableCollection<AxisPlacement> XAxisPlacements { get; }
        public ObservableCollection<AxisPlacement> YAxisPlacements { get;  }

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
            get { return _showBackgroundImage; }
            set { SetProperty(ref _showBackgroundImage, value); }
        }

        private bool _showGridLine;
        public bool ShowGridLine
        {
            get { return _showGridLine; }
            set { SetProperty(ref _showGridLine, value); }
        }

    }

    /// <summary>
    /// Interaction logic for SeriesTemplateDemo.xaml
    /// </summary>
    public partial class SeriesTemplateDemoView : UserControl
    {
        public SeriesTemplateDemoView()
        {
            InitializeComponent();
        }
    }
}
