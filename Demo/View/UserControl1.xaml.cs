using System;
using System.Collections.Generic;
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

namespace Demo
{
    public class SeriesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DataTemplate0 { get; set; }
        public DataTemplate DataTemplate1 { get; set; }
        public DataTemplate DataTemplate2 { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var obj = item as SomeSeries;
            if (obj == null)
            {
                return null;
            }

            switch (obj.Index)
            {
                case 0:
                    return DataTemplate0;
                case 1:
                    return DataTemplate1;
                case 2:
                    return DataTemplate2;
                default:
                    throw new IndexOutOfRangeException();
            }

        }
    }

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public DemoDataViewModel DemoDataViewModel { get; }
        public UserControl1()
        {
            InitializeComponent();
        }
    }
}
