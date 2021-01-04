using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MvvmCharting.WpfFX
{
    public class LegendItemControl : InteractiveControl
    {
        static LegendItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegendItemControl), new FrameworkPropertyMetadata(typeof(LegendItemControl)));
        }

    

        public Brush IndicatorBrush
        {
            get { return (Brush)GetValue(IndicatorBrushProperty); }
            set { SetValue(IndicatorBrushProperty, value); }
        }
        public static readonly DependencyProperty IndicatorBrushProperty =
            DependencyProperty.Register("IndicatorBrush", typeof(Brush), typeof(LegendItemControl), new PropertyMetadata(null));



   

  
    }
}