using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using MvvmCharting.Axis;
using MvvmCharting.Common;
using MvvmCharting.GridLine;

namespace MvvmCharting.WpfFX
{    
    /// <summary>
    /// Used to display the grid lines of a <see cref="Chart"/>
    /// </summary>
    [TemplatePart(Name = "sPART_HorizontalGridLines", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_VerticalGridLines", Type = typeof(Grid))]
    public class GridLineControl : Control, IGridLineControl
    {
        static GridLineControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridLineControl), new FrameworkPropertyMetadata(typeof(GridLineControl)));
        }

        private static readonly string sPART_HorizontalGridLines = "PART_HorizontalGridLines";
        private static readonly string sPART_VerticalGridLines = "PART_VerticalGridLines";

        #region GridLine

        private double[] _horizontalTickOffsets;
        private double[] _verticalTickOffsets;

        private Grid PART_HorizontalGridLines;
        private Grid PART_VerticalGridLines;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            this.PART_HorizontalGridLines = (Grid)this.GetTemplateChild(sPART_HorizontalGridLines);
            this.PART_VerticalGridLines = (Grid)this.GetTemplateChild(sPART_VerticalGridLines);
            if (this.PART_HorizontalGridLines != null)
            {

                UpdateHorizontalGridLines();
                this.PART_HorizontalGridLines.SetBinding(UIElement.VisibilityProperty,
                    new Binding(nameof(this.HorizontalGridLineVisibility)) { Source = this });
            }

            if (this.PART_VerticalGridLines != null)
            {
                UpdateVerticalGridLines();

                this.PART_VerticalGridLines.SetBinding(UIElement.VisibilityProperty,
                    new Binding(nameof(this.VerticalGridLineVisibility)) { Source = this });


            }


        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (this.IsLoaded)
            {
                UpdateHorizontalGridLines();
                UpdateVerticalGridLines();
            }

            UpdateHorizontalGridLines();
            UpdateVerticalGridLines();
        }

        private void SetGridLineBindings(Line line, Orientation orientation)
        {

            switch (orientation)
            {
                case Orientation.Horizontal:
                    line.SetBinding(Line.Y1Property, new Binding());
                    line.SetBinding(Line.Y2Property, new Binding());

                    line.SetBinding(Line.X2Property, new Binding(nameof(this.ActualWidth)) { Source = this });
                    line.SetBinding(Line.StyleProperty, new Binding(nameof(this.HorizontalGridLineStyle)) { Source = this });
                    break;

                case Orientation.Vertical:
                    line.SetBinding(Line.X1Property, new Binding());
                    line.SetBinding(Line.X2Property, new Binding());

                    line.SetBinding(Line.Y2Property, new Binding(nameof(this.ActualHeight)) { Source = this });
                    line.SetBinding(Line.StyleProperty, new Binding(nameof(this.VerticalGridLineStyle)) { Source = this });
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }


        }




        public Visibility HorizontalGridLineVisibility
        {
            get { return (Visibility)GetValue(HorizontalGridLineVisibilityProperty); }
            set { SetValue(HorizontalGridLineVisibilityProperty, value); }
        }
        public static readonly DependencyProperty HorizontalGridLineVisibilityProperty =
            DependencyProperty.Register("HorizontalGridLineVisibility", typeof(Visibility), typeof(GridLineControl), new PropertyMetadata(Visibility.Visible));


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




        private void DoUpdateGridLines(UIElementCollection target, double[] source, Orientation orientation)
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

            for (int i = 0; i < source.Length; i++)
            {
                var newValue = source[i];
                if (i < oldCt)
                {
                    ((Line)target[i]).DataContext = newValue;
                }
                else
                {
                    var line = new Line();
                    line.DataContext = newValue;
                    SetGridLineBindings(line, orientation);
                    target.Add(line);
                }
            }


        }

        private void UpdateHorizontalGridLines()
        {

            if (this.PART_HorizontalGridLines != null)
            {
                DoUpdateGridLines(this.PART_HorizontalGridLines.Children, this._horizontalTickOffsets, Orientation.Horizontal);

            }
        }

        private void UpdateVerticalGridLines()
        {
            if (this.PART_VerticalGridLines != null)
            {
                DoUpdateGridLines(this.PART_VerticalGridLines.Children, this._verticalTickOffsets, Orientation.Vertical);

            }

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
