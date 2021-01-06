using System.Windows;

namespace MvvmCharting.WpfFX.Series
{
    /// <summary>
    /// Represents a concrete line series type, which is basically a path
    /// whose Fill property is fixed to null. It is also the base class
    /// for the three specific line types: <see cref="PolyLineSeries"/>,
    /// <see cref="StepLineSeries"/> and <see cref="SplineSeries"/>
    /// </summary>
    public class LineSeries: PathSeries
    {
        static LineSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineSeries), new FrameworkPropertyMetadata(typeof(LineSeries)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.PART_Shape != null)
            {
                this.PART_Shape.Fill = null;
            }
  
   
           
        }


    }

    /// <summary>
    /// Represents a line series which use <see cref="PolylineGeometryBuilder"/> to
    /// create its path geometry.
    /// </summary>
    public class PolyLineSeries : LineSeries
    {
        static PolyLineSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PolyLineSeries), new FrameworkPropertyMetadata(typeof(PolyLineSeries)));
        }
    }

    /// <summary>
    /// Represents a line series which use <see cref="StepLineGeometryBuilder"/> to
    /// create its path geometry.
    /// </summary>
    public class StepLineSeries : LineSeries
    {
        static StepLineSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StepLineSeries), new FrameworkPropertyMetadata(typeof(StepLineSeries)));
        }
    }

    /// <summary>
    /// Represents a line series which use <see cref="SplineGeometryBuilder"/> to
    /// create its path geometry.
    /// </summary>
    public class SplineSeries : LineSeries
    {
        static SplineSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplineSeries), new FrameworkPropertyMetadata(typeof(SplineSeries)));
        }
    }

}