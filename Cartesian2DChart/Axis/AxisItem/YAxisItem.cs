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

namespace MvvmCharting.Axis
{

    public class YAxisItem : AxisItem
    {
        static YAxisItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(YAxisItem), new FrameworkPropertyMetadata(typeof(YAxisItem)));
        }


        internal override void TryDoTranslateTransform()
        {
            double y = this.Position - (this.ActualHeight / 2 - this.PART_Tick?.ActualHeight ?? double.NaN / 2);
            if (y.IsNaN())
            {
                return;
            }

            if (this.RenderTransform is TranslateTransform)
            {
                ((TranslateTransform)this.RenderTransform).Y = y;
            }
            else
            {
                this.RenderTransform = new TranslateTransform(0, y);
            }

        }
    }
}
