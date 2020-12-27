namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// A handy class to draw a closed spline path.
    /// </summary>
    public class SplineAreaSeries : PathSeries
    {
        public SplineAreaSeries()
        {
            this.GeometryBuilder = new BezierAreaGeometryBuilder();
        }

    }
}