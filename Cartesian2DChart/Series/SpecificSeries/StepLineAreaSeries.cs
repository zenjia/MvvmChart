namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// A handy class to draw step poly line series.
    /// </summary>
    public class StepLineAreaSeries : PathSeries
    {
        public StepLineAreaSeries()
        {
            this.GeometryBuilder = new StepLineAreaGeometryBuilder();
        }



    }
}