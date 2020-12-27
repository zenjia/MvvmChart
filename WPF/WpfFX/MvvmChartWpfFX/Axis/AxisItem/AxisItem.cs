using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MvvmCharting.Axis;
using MvvmCharting.Common;

namespace MvvmCharting.WpfFX.Axis
{
    /// <summary>
    /// Represents an item of an axis, typically including a label and a tick.
    /// </summary>
    [TemplatePart(Name = "PART_Tick", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_Label", Type = typeof(TextBlock))]
    public class AxisItem : Control, IAxisItem
    {
        static AxisItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AxisItem), new FrameworkPropertyMetadata(typeof(AxisItem)));
        }


        private static readonly string sPART_Tick = "PART_Tick";
        private static readonly string sPART_Label = "PART_Label";
        protected FrameworkElement PART_Tick { get; private set; }
        private FrameworkElement PART_Label;
        public AxisItem()
        {
            this.SetBinding(CoordinateProperty, new Binding(nameof(IAxisItemDrawingBaseParams.Coordinate)));
            this.SetBinding(ValueProperty, new Binding(nameof(IAxisItemDrawingBaseParams.Value)));
        }



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_Tick = (FrameworkElement)this.GetTemplateChild(sPART_Tick);

            this.PART_Label = (TextBlock)this.GetTemplateChild(sPART_Label);
            if (this.PART_Label != null)
            {
                this.PART_Label.SetBinding(TextBlock.TextProperty, new Binding(nameof(this.LabelText)) { Source = this });
            }
            this.TryDoTranslateTransform();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.TryDoTranslateTransform();
        }


 

        public AxisPlacement Placement
        {
            get { return (AxisPlacement)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(AxisPlacement), typeof(AxisItem), new PropertyMetadata(AxisPlacement.Bottom));



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


        public double Coordinate
        {
            get { return (double)this.GetValue(CoordinateProperty); }
            set { this.SetValue(CoordinateProperty, value); }
        }
        public static readonly DependencyProperty CoordinateProperty =
            DependencyProperty.Register("Coordinate", typeof(double), typeof(AxisItem), new PropertyMetadata(double.NaN, OnPositionPropertyChanged));

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisItem)d).OnPositionChanged();
        }

        private void OnPositionChanged()
        {
            this.TryDoTranslateTransform();
        }

        public void SetLabelTextConverter(IValueConverterNS newValue)
        {
            this.LabelTextConverter = newValue;
        }

        public void SetAxisPlacement(AxisPlacement newValue)
        {
            this.Placement = newValue;
        }

        public IValueConverterNS LabelTextConverter
        {
            get { return (IValueConverterNS)this.GetValue(LabelTextConverterProperty); }
            set { this.SetValue(LabelTextConverterProperty, value); }
        }
        public static readonly DependencyProperty LabelTextConverterProperty =
            DependencyProperty.Register("LabelTextConverter", typeof(IValueConverterNS), typeof(AxisItem), new PropertyMetadata(null, OnValueConverterPropertyChanged));

        private static void OnValueConverterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisItem)d).UpdateLabelText();
        }


        private void UpdateLabelText()
        {
            if (this.LabelTextConverter == null || this.Value == null)
            {
                return;
            }
 
            this.LabelText = this.LabelTextConverter.ConverterTo(this.Value, CultureInfo.CurrentCulture)?.ToString() ?? string.Empty;
        }



        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(AxisItem), new PropertyMetadata(null, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisItem)d).UpdateLabelText();
        }





        public string LabelText
        {
            get { return (string)this.GetValue(LabelTextProperty); }
            set { this.SetValue(LabelTextProperty, value); }
        }
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(AxisItem), new PropertyMetadata(null));


        private double GetAdjustedCoordinate()
        {

            double totalLength;
            double tickLength;


            switch (this.Placement)
            {
                case AxisPlacement.Top:
                case AxisPlacement.Bottom:

                    totalLength = this.ActualWidth;
                    tickLength = this.PART_Tick?.ActualWidth ?? 0;
                    break;

                case AxisPlacement.Left:
                case AxisPlacement.Right:
                    totalLength = this.ActualHeight;
                    tickLength = this.PART_Tick?.ActualHeight ?? 0;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            double x = this.Coordinate - (totalLength - tickLength) / 2;

            return x;
        }

        private void TryDoTranslateTransform()
        {
            double x = GetAdjustedCoordinate();
            if (x.IsNaN() || double.IsInfinity(x))
            {
                return;
            }

            var old = this.RenderTransform as TranslateTransform;
            switch (this.Placement)
            {
                case AxisPlacement.Top:
                case AxisPlacement.Bottom:

                    if (old != null)
                    {
                        old.X = x;
                    }
                    else
                    {
                        this.RenderTransform = new TranslateTransform(x, 0);
                    }

                    break;

                case AxisPlacement.Left:
                case AxisPlacement.Right:

                    if (old != null)
                    {
                        old.Y = x;
                    }
                    else
                    {
                        this.RenderTransform = new TranslateTransform(0, x);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }




    }
}