using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MvvmCharting.WpfFX
{
    public class LegendItemControl : Control
    {
        static LegendItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegendItemControl), new FrameworkPropertyMetadata(typeof(LegendItemControl)));
        }

        public event Action<object, string> PropertyChanged;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == Control.IsMouseOverProperty)
            {
                this.IsHighlighted = this.IsMouseOver;
            }
            else if (e.Property == LegendItemControl.IsHighlightedProperty)
            {
                this.PropertyChanged?.Invoke(this, nameof(this.IsHighlighted));
            }
        }


        public Brush IndicatorBrush
        {
            get { return (Brush)GetValue(IndicatorBrushProperty); }
            set { SetValue(IndicatorBrushProperty, value); }
        }
        public static readonly DependencyProperty IndicatorBrushProperty =
            DependencyProperty.Register("IndicatorBrush", typeof(Brush), typeof(LegendItemControl), new PropertyMetadata(null));



        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }
        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(LegendItemControl), new PropertyMetadata(false));




    }
}