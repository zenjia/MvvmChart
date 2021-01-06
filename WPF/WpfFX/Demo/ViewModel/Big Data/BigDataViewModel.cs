using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MvvmCharting.Common;
using MvvmCharting;

namespace Demo
{
    public class BigDataViewModel : DemoDataViewModel
    {
         

        public BigDataViewModel()
          : base()
        {


            this.Max = 1000;

            this.SeriesCount = 1;

            this.ShowScatterSeries = false;
            //RefreshData();

        }


    }
}