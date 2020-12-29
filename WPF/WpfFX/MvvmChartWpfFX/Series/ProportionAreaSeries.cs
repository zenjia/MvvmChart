using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using MvvmCharting.Drawing;

namespace MvvmCharting.WpfFX
{
    public class ProportionAreaSeries : PathSeries
    {
        internal ISeriesHost Owner { get; set; }

        private Dictionary<object, int> itemIndexMap = new Dictionary<object, int>();
        protected override void HandleItemsSourceCollectionChange(IList oldValue, IList newValue)
        {
            if (newValue != null)
            {
                if (newValue.Count > 1)
                {
                    this.itemIndexMap.Clear();
                    for (int i = 0; i < this.ItemsSource.Count; i++)
                    {
                        this.itemIndexMap.Add(this.ItemsSource[i], i);
                    }
                }
                else
                {
                    var newItem = newValue[0];
                    var index = this.ItemsSource.IndexOf(newItem);
                    this.itemIndexMap.Add(newItem, index);
                }
            }

            base.HandleItemsSourceCollectionChange(oldValue, newValue);
        }

        protected override PointNS GetPlotCoordinateForItem(object item)
        {
            if (this.Owner.GetSeries().Any(x => x.ItemsSource == null))
            {
                return default(PointNS);
            }

          

            var index = this.itemIndexMap[item];

            

            double total = 0;
            double sum = 0;
            bool meet = false;
             
            foreach (var series in this.Owner.GetSeries())
            {
                var yValue = GetYValueFromItem(series.ItemsSource[index]);

                total += yValue;

                if (!meet)
                {
                    sum += yValue;
                }

                if (series == this)
                {
                    meet = true;
                }

            }

            var y = sum / total;
 

            var itemValue = GetValueFromItem(item);
            var pt = new PointNS((itemValue.X - this.PlottingXValueRange.Min) * this.xPixelPerUnit,
                (y - this.PlottingYValueRange.Min) * this.yPixelPerUnit);

            return pt;
        }

        protected override PointNS[] GetPreviousSeriesCoordinates()
        {
            var ls = this.Owner.GetSeries().ToList();
            var index = ls.IndexOf(this);
            PointNS[] previous;
            if (index == 0)
            {
                previous = new PointNS[2];
                previous[0] = new PointNS(0, 0);
                previous[1] = new PointNS(this.ActualWidth, 0);
            }
            else
            {
 
                previous = ls[index - 1].GetCoordinates();
            }

            return previous;
        }
    }
}