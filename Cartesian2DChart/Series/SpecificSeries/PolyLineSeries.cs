namespace MvvmCharting
{
    /// <summary>
    /// A handy class to draw poly line series.
    /// </summary>
    public class PolyLineSeries : PathSeries
    {
        public PolyLineSeries()
        {
            this.GeometryBuilder = new PolyLineGeometryBuilder();
        }

    }
}