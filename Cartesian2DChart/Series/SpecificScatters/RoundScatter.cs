namespace MvvmCharting
{
    public class RoundScatter: Scatter
    {
        public double Radios { get; set; } = 6.0;
        public RoundScatter()
        {
            var ellipseGeometryBuilder = new EllipseGeometryBuilder();
            ellipseGeometryBuilder.RadiusX = Radios;
            ellipseGeometryBuilder.RadiusY = Radios;
            this.GeometryBuilder = ellipseGeometryBuilder;
        }
    }
}
