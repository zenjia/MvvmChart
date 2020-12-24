namespace MvvmCharting
{
    public class PolyLineAreaSeries : PathSeries
    {
        public PolyLineAreaSeries()
        {
            this.GeometryBuilder = new PolylineAreaGeometryBuilder();
        }

    }
}