using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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

namespace WpfApp1
{
 
 

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> List { get; private set; }
        public MainWindow()
        {
            List = new List<string>();
            for (int i = 0; i < 9; i++)
            {
                List.Add(i.ToString());
            }
            InitializeComponent();
        }


        private int i = 5;
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Border.RenderTransform = new TranslateTransform(this.i* 10, this.i * 10);
            mytrans.X = this.i * 10;
            //this.mytrans.Y= this.i * 10;

            this.i++;
        }
    }
}
