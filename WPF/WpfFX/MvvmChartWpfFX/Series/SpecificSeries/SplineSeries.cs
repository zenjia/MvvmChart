namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// A handy class to draw spline series.
    /// </summary>
    public class SplineSeries : PathSeries
    {
        public SplineSeries()
        {
            this.GeometryBuilder = new BezierGeometryBuilder();
        }

    }
}