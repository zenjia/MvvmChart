using System;
using System.Collections.Generic;
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

namespace MvvmCharting.Axis
{
    public class XAxisItem : AxisItem
    {
        static XAxisItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XAxisItem), new FrameworkPropertyMetadata(typeof(XAxisItem)));
        }
 

        internal override void TryDoTranslateTransform()
        {
            double x = this.Position - (this.ActualWidth / 2 - this.PART_Tick?.ActualWidth??double.NaN / 2);
            if (x.IsNaN())
            {
                return;
            }

            if (this.RenderTransform is TranslateTransform)
            {
                ((TranslateTransform)this.RenderTransform).X = x;
            }
            else
            {
                this.RenderTransform = new TranslateTransform(x, 0);
            }

        }
    }
}
