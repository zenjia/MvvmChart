using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;

namespace MvvmCharting.WpfFX.Series
{
    /// <summary>
    /// Represents area series, in fact they are just <see cref="LineSeries"/> which is filled
    /// by color.It is also the base class
    /// for three specific line types: <see cref="PolyLineAreaSeries"/>,
    /// <see cref="StepLineAreaSeries"/> and <see cref="SplineAreaSeries"/>
    /// </summary>
    public class AreaSeries : PathSeries
    {
        static AreaSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AreaSeries), new FrameworkPropertyMetadata(typeof(AreaSeries)));
        }

        public AreaSeries()
        {
            this.IsAreaMode = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.PART_Shape != null)
            {
                this.PART_Shape.SetBinding(Shape.FillProperty, new Binding(nameof(this.Stroke)) { Source = this });

            }
        }


    }


    /// <summary>
    /// Represents a line series which use <see cref="PolyLineGeometryBuilder"/> to
    /// create its path geometry.
    /// </summary>
    public class PolyLineAreaSeries : AreaSeries
    {

 
        static PolyLineAreaSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PolyLineAreaSeries), new FrameworkPropertyMetadata(typeof(PolyLineAreaSeries)));
        }

 
    }

    /// <summary>
    /// Represents a line series which use <see cref="StepLineGeometryBuilder"/> to
    /// create its path geometry.
    /// </summary>
    public class StepLineAreaSeries : AreaSeries
    {
        static StepLineAreaSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StepLineAreaSeries), new FrameworkPropertyMetadata(typeof(StepLineAreaSeries)));
        }
    }

    /// <summary>
    /// Represents a line series which use <see cref="SplineGeometryBuilder"/> to
    /// create its path geometry.
    /// </summary>
    public class SplineAreaSeries : AreaSeries
    {
        static SplineAreaSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplineAreaSeries), new FrameworkPropertyMetadata(typeof(SplineAreaSeries)));
        }
    }

}