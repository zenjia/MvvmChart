using System;
using System.Windows;
using System.Windows.Controls;

namespace Demo
{
    public class SeriesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DataTemplate0 { get; set; }
        public DataTemplate DataTemplate1 { get; set; }
        public DataTemplate DataTemplate2 { get; set; }
 

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var obj = item as SomePointList;
            if (obj == null)
            {
                return null;
            }

            switch (obj.Index)
            {
                case 0:
                    return this.DataTemplate0;
                case 1:
                    return this.DataTemplate1;
                case 2:
                    return this.DataTemplate2;
 

                default:
                    throw new IndexOutOfRangeException();
            }

        }
    }
}