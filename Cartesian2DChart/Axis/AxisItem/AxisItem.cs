using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MvvmChart.Common;

namespace MvvmCharting.Axis
{
    /// <summary>
    /// The item of an Axis indicates the coordinate, typically including a label and a tick.
    /// </summary>
    [TemplatePart(Name = "PART_Tick", Type = typeof(FrameworkElement))]
    // [TemplatePart(Name = "PART_Label", Type = typeof(TextBlock))]
    public class AxisItem : Control
    {
        static AxisItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AxisItem), new FrameworkPropertyMetadata(typeof(AxisItem)));
        }


        private static readonly string sPART_Tick = "PART_Tick";
        //private static readonly string sPART_Label = "PART_Label";
        protected FrameworkElement PART_Tick { get; private set; }
        //private FrameworkElement PART_Label;
        public AxisItem()
        {

            Binding b = new Binding(nameof(AxisItemDrawingParam.Coordinate));
            this.SetBinding(CoordinateProperty, b);

            
            this.UpdateLabelTextBinding();

        }



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_Tick = (FrameworkElement)this.GetTemplateChild(sPART_Tick);
            //this.PART_Label = (TextBlock)this.GetTemplateChild(sPART_Label);
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

            Binding b = new Binding(nameof(AxisItemDrawingParam.Value));
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