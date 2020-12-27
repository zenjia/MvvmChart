namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// a specific Scatter type
    /// </summary>
    public class RoundScatter2: Scatter2
    {
        public double Radios { get; set; } = 6.0;
        public RoundScatter2()
        {
            var ellipseGeometryBuilder = new EllipseGeometryBuilder();
            ellipseGeometryBuilder.RadiusX = Radios;
            ellipseGeometryBuilder.RadiusY = Radios;
            this.GeometryBuilder = ellipseGeometryBuilder;
        }
    }
}
