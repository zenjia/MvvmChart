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
    /// <summary>
    /// The item of an Axis indicates the coordinate, typically including a label and a tick.
    /// </summary>
    [TemplatePart(Name = "PART_Tick", Type = typeof(FrameworkElement))]
   // [TemplatePart(Name = "PART_Label", Type = typeof(TextBlock))]
    public abstract class AxisItem : Control
    {
        private static readonly string sPART_Tick = "PART_Tick";
        //private static readonly string sPART_Label = "PART_Label";
        protected FrameworkElement PART_Tick { get; private set; }
        //private FrameworkElement PART_Label;
        protected AxisItem()
        {

            Binding b = new Binding(nameof(DataOffset.Offset));
            this.SetBinding(PositionProperty, b);

            b = new Binding(nameof(DataOffset.Data));
            this.SetBinding(DataValueProperty, b);

            this.UpdateLabelTextBinding();
            this.SizeChanged += this.AxisItem_SizeChanged;
        }



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_Tick = (FrameworkElement)this.GetTemplateChild(sPART_Tick);
            //this.PART_Label = (TextBlock)this.GetTemplateChild(sPART_Label);
            this.TryDoTranslateTransform();
        }

        private void AxisItem_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.TryDoTranslateTransform();
        }

        public double TickLength
        {
            get { return (double)this.GetValue(TickLengthProperty); }
            set { this.SetValue(TickLengthProperty, value); }
        }
        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register("TickLength", typeof(double), typeof(AxisItem), new PropertyMetadata(8.0));

        public Brush TickStroke
        {
            get { return (Brush)this.GetValue(TickStrokeProperty); }
            set { this.SetValue(TickStrokeProperty, value); }
        }
        public static readonly DependencyProperty TickStrokeProperty =
            DependencyProperty.Register("TickStroke", typeof(Brush), typeof(AxisItem), new PropertyMetadata(Brushes.Gray));


        public double Position
        {
            get { return (double)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(double), typeof(AxisItem), new PropertyMetadata(double.NaN, OnPositionPropertyChanged));

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisItem)d).OnPositionChanged();
        }

        private void OnPositionChanged()
        {
            this.TryDoTranslateTransform();
        }


        public double DataValue
        {
            get { return (double)this.GetValue(DataValueProperty); }
            set { this.SetValue(DataValueProperty, value); }
        }
        public static readonly DependencyProperty DataValueProperty =
            DependencyProperty.Register("DataValue", typeof(double), typeof(AxisItem), new PropertyMetadata(double.NaN));


        public IValueConverter LabelTextConverter
        {
            get { return (IValueConverter)this.GetValue(LabelTextConverterProperty); }
            set { this.SetValue(LabelTextConverterProperty, value); }
        }
        public static readonly DependencyProperty LabelTextConverterProperty =
            DependencyProperty.Register("LabelTextConverter", typeof(IValueConverter), typeof(AxisItem), new PropertyMetadata(null, OnValueConverterPropertyChanged));

        private static void OnValueConverterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisItem)d).UpdateLabelTextBinding();
        }

        private void UpdateLabelTextBinding()
        {

 

            BindingOperations.ClearBinding(this, LabelTextProperty);

            Binding b = new Binding(nameof(DataOffset.Data));
            if (this.LabelTextConverter != null)
            {
                b.Converter = this.LabelTextConverter;
            }

            this.SetBinding(LabelTextProperty, b);
        }


        public string LabelText
        {
            get { return (string)this.GetValue(LabelTextProperty); }
            set { this.SetValue(LabelTextProperty, value); }
        }
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(AxisItem), new PropertyMetadata(null));




        internal abstract void TryDoTranslateTransform();




    }
}