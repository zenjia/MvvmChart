using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
    public class ItemToScatterFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }



            SomePoint pt = (SomePoint)value;
            int i = (int)pt.t;
            int j = (int)pt.Y;

            if (i % 2 == 0 && j % 2 == 0)
            {
                return Brushes.Red;
            }


            return Brushes.LightSeaGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

     
    }

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

        private bool _showGridLine = true;
        public bool ShowGridLine
        {
            get { return _showGridLine; }
            set { SetProperty(ref _showGridLine, value); }
        }

        private bool _showLegend = true;
        public bool ShowLegend
        {
            get { return _showLegend; }
            set { SetProperty(ref _showLegend, value); }
        }

    }

    /// <summary>
    /// Interaction logic for SeriesTemplateDemo.xaml
    /// </summary>
    public partial class GeneralDemoView : UserControl
    {
        public GeneralDemoView()
        {
            InitializeComponent();
        }

        private void btnAddDataClick(object sender, RoutedEventArgs e)
        {
            DemoDataViewModel viewModel = (DemoDataViewModel) this.Resources["DemoDataViewModel"];
            viewModel.AddData();
        }

        private void btnRemoveDataClick(object sender, RoutedEventArgs e)
        {
            DemoDataViewModel viewModel = (DemoDataViewModel)this.Resources["DemoDataViewModel"];
            viewModel.RemoveData();
        }
    }
}
