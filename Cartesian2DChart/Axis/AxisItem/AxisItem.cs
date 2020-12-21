using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MvvmCharting.Axis
{
    [TemplatePart(Name = "PART_Tick", Type = typeof(FrameworkElement))]
    public abstract class AxisItem : Control
    {
        private static readonly string sPART_Tick = "PART_Tick";
        protected FrameworkElement PART_Tick { get; private set; }

        protected AxisItem()
        {
            
            Binding b = new Binding(nameof(DataOffset.Offset));
            SetBinding(PositionProperty, b);

            b = new Binding(nameof(DataOffset.Data));
            SetBinding(DataValueProperty, b);

            UpdateLabelTextBinding();
            this.SizeChanged += AxisItem_SizeChanged;
        }

        

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_Tick = (FrameworkElement)GetTemplateChild(sPART_Tick);
            TryDoTranslateTransform();
        }

        private void AxisItem_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TryDoTranslateTransform();
        }

        public double TickLength
        {
            get { return (double)GetValue(TickLengthProperty); }
            set { SetValue(TickLengthProperty, value); }
        }
        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register("TickLength", typeof(double), typeof(AxisItem), new PropertyMetadata(8.0));

        public Brush TickStroke
        {
            get { return (Brush)GetValue(TickStrokeProperty); }
            set { SetValue(TickStrokeProperty, value); }
        }
        public static readonly DependencyProperty TickStrokeProperty =
            DependencyProperty.Register("TickStroke", typeof(Brush), typeof(AxisItem), new PropertyMetadata(Brushes.Gray));


        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(double), typeof(AxisItem), new PropertyMetadata(double.NaN, OnPositionPropertyChanged));

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisItem)d).OnPositionChanged();
        }

        private void OnPositionChanged()
        {
            TryDoTranslateTransform();
        }


        public double DataValue
        {
            get { return (double)GetValue(DataValueProperty); }
            set { SetValue(DataValueProperty, value); }
        }
        public static readonly DependencyProperty DataValueProperty =
            DependencyProperty.Register("DataValue", typeof(double), typeof(AxisItem), new PropertyMetadata(double.NaN));

 
        public IValueConverter ValueConverter
        {
            get { return (IValueConverter)GetValue(ValueConverterProperty); }
            set { SetValue(ValueConverterProperty, value); }
        }
        public static readonly DependencyProperty ValueConverterProperty =
            DependencyProperty.Register("ValueConverter", typeof(IValueConverter), typeof(AxisItem), new PropertyMetadata(null, OnValueConverterPropertyChanged));

        private static void OnValueConverterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisItem)d).UpdateLabelTextBinding();
        }

        private void UpdateLabelTextBinding()
        {
 
            BindingOperations.ClearBinding(this, LabelTextProperty);

            Binding b = new Binding(nameof(DataOffset.Data));
            if (this.ValueConverter != null)
            {
                b.Converter = this.ValueConverter;
            }
 
            SetBinding(LabelTextProperty, b);
        }


        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(AxisItem), new PropertyMetadata(null));




        internal abstract void TryDoTranslateTransform();




    }
}