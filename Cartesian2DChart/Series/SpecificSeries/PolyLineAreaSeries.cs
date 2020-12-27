namespace MvvmCharting.WpfFX
{
    public class PolyLineAreaSeries : PathSeries
    {
        public PolyLineAreaSeries()
        {
            this.GeometryBuilder = new PolylineAreaGeometryBuilder();
        }

    }
}