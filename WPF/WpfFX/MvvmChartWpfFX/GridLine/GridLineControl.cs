using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using MvvmCharting.Axis;
using MvvmCharting.Common;
using MvvmCharting.GridLine;

namespace MvvmCharting.WpfFX
{
    [TemplatePart(Name = "PART_HorizontalGridLineItemsControl", Type = typeof(SlimItemsControl))]
    [TemplatePart(Name = "PART_VerticalGridLineItemsControl", Type = typeof(SlimItemsControl))]
    public class GridLineControl : Control, IGridLineControl
    {
        static GridLineControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridLineControl), new FrameworkPropertyMetadata(typeof(GridLineControl)));
        }

        private static readonly string sPART_HorizontalGridLineItemsControl = "PART_HorizontalGridLineItemsControl";
        private static readonly string sPART_VerticalGridLineItemsControl = "PART_VerticalGridLineItemsControl";

        #region GridLine

        private double[] _horizontalTickOffsets;
        private double[] _verticalTickOffsets;
        private SlimItemsControl PART_HorizontalGridLineItemsControl;
        private SlimItemsControl PART_VerticalGridLineItemsControl;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.PART_HorizontalGridLineItemsControl != null)
            {
                this.PART_HorizontalGridLineItemsControl.ElementGenerated -= HorizontalGridLineItemTemplateApplied;
            }

            if (this.PART_VerticalGridLineItemsControl != null)
            {
                this.PART_VerticalGridLineItemsControl.ElementGenerated += VerticalGridLineItemTemplateApplied;
            }

            this.PART_HorizontalGridLineItemsControl = (SlimItemsControl)GetTemplateChild(sPART_HorizontalGridLineItemsControl);
            this.PART_VerticalGridLineItemsControl = (SlimItemsControl)GetTemplateChild(sPART_VerticalGridLineItemsControl);

            if (this.PART_HorizontalGridLineItemsControl!=null)
            {
                this.PART_HorizontalGridLineItemsControl.ItemsSource = this.HorizontalGridLineOffsets;
                this.PART_HorizontalGridLineItemsControl.ElementGenerated += HorizontalGridLineItemTemplateApplied;

                this.PART_HorizontalGridLineItemsControl.SetBinding(UIElement.VisibilityProperty,
                    new Binding(nameof(this.HorizontalGridLineVisiblility)) { Source = this});
            }

            if (this.PART_VerticalGridLineItemsControl!=null)
            {
                this.PART_VerticalGridLineItemsControl.ItemsSource = this.VerticalGridLineOffsets;
                this.PART_VerticalGridLineItemsControl.SetBinding(UIElement.VisibilityProperty,
                    new Binding(nameof(this.VerticalGridLineVisibility)) { Source = this });

                this.PART_VerticalGridLineItemsControl.ElementGenerated += VerticalGridLineItemTemplateApplied;

            }
          
           
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!this.IsLoaded)
            {
                return;
            }

            UpdateHorizontalGridLines();
            UpdateVerticalGridLines();
        }

  
   

 

        private void SetGridLineBindings(DependencyObject rootChild, Orientation orientation)
        {
            if (!(rootChild is Line))
            {
                throw new MvvmChartException($"The {orientation} grid line should be of type: Line!");
            }

            
            Line line = (Line)rootChild;
            Binding b;
            switch (orientation)
            {
                case Orientation.Horizontal:
                    line.SetBinding(Line.Y1Property, new Binding());
                    line.SetBinding(Line.Y2Property, new Binding());

                    b = new Binding(nameof(this.ActualWidth));
                    b.Source = this;
                    line.SetBinding(Line.X2Property, b);

                    b = new Binding(nameof(this.HorizontalGridLineStyle));
                    b.Source = this;
                    break;

                case Orientation.Vertical:
                    line.SetBinding(Line.X1Property, new Binding());
                    line.SetBinding(Line.X2Property, new Binding());

                    b = new Binding(nameof(this.ActualHeight));
                    b.Source = this;
                    line.SetBinding(Line.Y2Property, b);
                    
                    b = new Binding(nameof(this.VerticalGridLineStyle));
                    b.Source = this;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }

            line.SetBinding(StyleProperty, b);
        }
         
        private void VerticalGridLineItemTemplateApplied(object arg1, DependencyObject rootChild, int index)
        {
            SetGridLineBindings(rootChild, Orientation.Vertical);
        }

        private void HorizontalGridLineItemTemplateApplied(object arg1, DependencyObject rootChild, int index)
        {
            SetGridLineBindings(rootChild, Orientation.Horizontal);
        }




        public Visibility HorizontalGridLineVisiblility
        {
            get { return (Visibility)GetValue(HorizontalGridLineVisiblilityProperty); }
            set { SetValue(HorizontalGridLineVisiblilityProperty, value); }
        }
        public static readonly DependencyProperty HorizontalGridLineVisiblilityProperty =
            DependencyProperty.Register("HorizontalGridLineVisiblility", typeof(Visibility), typeof(GridLineControl), new PropertyMetadata(Visibility.Visible));



        public Visibility VerticalGridLineVisibility
        {
            get { return (Visibility)GetValue(VerticalGridLineVisibilityProperty); }
            set { SetValue(VerticalGridLineVisibilityProperty, value); }
        }
        public static readonly DependencyProperty VerticalGridLineVisibilityProperty =
            DependencyProperty.Register("VerticalGridLineVisibility", typeof(Visibility), typeof(GridLineControl), new PropertyMetadata(Visibility.Visible));




        public Style HorizontalGridLineStyle
        {
            get { return (Style)GetValue(HorizontalGridLineStyleProperty); }
            set { SetValue(HorizontalGridLineStyleProperty, value); }
        }
        public static readonly DependencyProperty HorizontalGridLineStyleProperty =
            DependencyProperty.Register("HorizontalGridLineStyle", typeof(Style), typeof(GridLineControl), new PropertyMetadata(null));


        public Style VerticalGridLineStyle
        {
            get { return (Style)GetValue(VerticalGridLineStyleProperty); }
            set { SetValue(VerticalGridLineStyleProperty, value); }
        }
        public static readonly DependencyProperty VerticalGridLineStyleProperty =
            DependencyProperty.Register("VerticalGridLineStyle", typeof(Style), typeof(GridLineControl), new PropertyMetadata(null));








        public ObservableCollection<double> HorizontalGridLineOffsets { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> VerticalGridLineOffsets { get; } = new ObservableCollection<double>();


        private void DoUpdateGridLines(ObservableCollection<double> target, double[] source)
        {
            if (source == null)
            {
                return;
            }

 
            var newCt = source.Length;
            var oldCt = target.Count;
            if (oldCt > newCt)
            {
                target.RemoveRange(newCt, oldCt - newCt);
            }
            else
            {
                for (int i = 0; i < source.Length; i++)
                {
                    var newValue = source[i];
                    if (i < oldCt)
                    {
                        if (!target[i].NearlyEqual(newValue, 0.01))
                        {
                            target[i] = newValue;
                        }
                       
                    }
                    else
                    {
                        target.Add(newValue);
                    }
                }
            }

        }

        private void UpdateHorizontalGridLines()
        {


            DoUpdateGridLines(this.HorizontalGridLineOffsets, this._horizontalTickOffsets);
        }

        private void UpdateVerticalGridLines()
        {

            DoUpdateGridLines(this.VerticalGridLineOffsets, this._verticalTickOffsets);
        }
        #endregion


        public void OnAxisItemCoordinateChanged(AxisType orientation, IEnumerable<double> ticks)
        {
            if (ticks == null)
            {
                return;
            }

            switch (orientation)
            {
                case AxisType.Y:
                    this._horizontalTickOffsets = ticks.ToArray();
                    UpdateHorizontalGridLines();
                    break;
                case AxisType.X:
                    this._verticalTickOffsets = ticks.ToArray();
                    UpdateVerticalGridLines();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }


        }
    }
}
