using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Demo
{
 

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public DemoDataViewModel DemoDataViewModel { get; }

        public MainWindow()
        {
           
            InitializeComponent();

            this.DemoDataViewModel = (DemoDataViewModel)Application.Current.Resources["GlobalDemoDataViewModel"];
        }




 
    }
}
