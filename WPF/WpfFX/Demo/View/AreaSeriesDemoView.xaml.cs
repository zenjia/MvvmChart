using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MvvmCharting.Common;

namespace Demo
{
    public class DemoDataViewModel2 : BindableBase
    {

        public ObservableCollection<SomePointList> ItemsSourceList { get; }

     

        private SomePoint GetPoint(int i)
        {
            var v = i / 1.0;
            var y = Math.Abs(v) < 1e-10 ? 1 : Math.Sin(v) / v;
            var pt = new SomePoint(v, y+0.5);

            return pt;
        }

        private Random _random = new Random();

        private void InitiateData()
        {
            var ls = new SomePointList(0);
            ls.DataList.Add(new SomePoint(1,38));
            ls.DataList.Add(new SomePoint(2, 32));
            ls.DataList.Add(new SomePoint(3, 22));
            ls.DataList.Add(new SomePoint(4, 42));
            ls.DataList.Add(new SomePoint(5, 62));
            ls.DataList.Add(new SomePoint(6, 32));
            this.ItemsSourceList.Add(ls);

            ls = new SomePointList(1);
            ls.DataList.Add(new SomePoint(1, 18));
            ls.DataList.Add(new SomePoint(2, 36));
            ls.DataList.Add(new SomePoint(3, 52));
            ls.DataList.Add(new SomePoint(4, 42));
            ls.DataList.Add(new SomePoint(5, 32));
            ls.DataList.Add(new SomePoint(6, 39));
            this.ItemsSourceList.Add(ls);

            ls = new SomePointList(2);
            ls.DataList.Add(new SomePoint(1, 48));
            ls.DataList.Add(new SomePoint(2, 62));
            ls.DataList.Add(new SomePoint(3, 22));
            ls.DataList.Add(new SomePoint(4, 22));
            ls.DataList.Add(new SomePoint(5, 12));
            ls.DataList.Add(new SomePoint(6, 37));
            this.ItemsSourceList.Add(ls);

            return;
            var first = new SomePointList(0);
            for (int i = 0; i <= 30; i++)
            {
                var pt = GetPoint(i);
                first.DataList.Add(pt);
            }

            this.ItemsSourceList.Add(first);

            for (int i = 1; i < 3; i++)
            {
                var list = new SomePointList(i);
                double yOffset = i * (_random.NextDouble() - 0.5);
                foreach (var item in first.DataList)
                {
                    list.DataList.Add(new SomePoint(item.t, item.Y + yOffset));
                }

                ItemsSourceList.Insert(0, list);
            }
        }

      


        public DemoDataViewModel2()
        {
 


            this.ItemsSourceList = new ObservableCollection<SomePointList>();
 
            InitiateData();





        }
    }

    /// <summary>
    /// Interaction logic for AreaSeriesDemoView.xaml
    /// </summary>
    public partial class AreaSeriesDemoView : UserControl
    {
        public AreaSeriesDemoView()
        {
            InitializeComponent();
        }
    }
}
