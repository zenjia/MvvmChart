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
using MvvmCharting;

namespace Demo
{
    public class MyScatterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DataTemplate0 { get; set; }
        public DataTemplate DataTemplate1 { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }
             
            SomePoint pt = (SomePoint)item;
            int i = (int)pt.t;
     

            if (i%2==0 )
            {
                return DataTemplate1;
            }


            return DataTemplate0;
        }
    }

    /// <summary>
    /// Interaction logic for ScatterCustomizationDemoView.xaml
    /// </summary>
    public partial class ScatterCustomizationDemoView : UserControl
    {
        public ScatterCustomizationDemoView()
        {
            InitializeComponent();
        }
    }
}
