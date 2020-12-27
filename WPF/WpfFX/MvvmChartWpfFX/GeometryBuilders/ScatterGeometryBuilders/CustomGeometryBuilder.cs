using System.Windows.Media;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// Customizable GeometryBuilder which can accept a mini-language string,
    /// and build a Geometry by calling Geometry<see cref="GetGeometry"/>.Parse() method.
    /// </summary>
    public class CustomGeometryBuilder : IScatterGeometryBuilder
    {
        private Geometry _cachedGeometry;

        private string _rawData;
        public string RawData
        {
            get { return this._rawData; }
            set
            {
                if (this._rawData != value)
                {
                    this._rawData = value;

                    this._cachedGeometry = string.IsNullOrEmpty(this.RawData) ? Geometry.Empty : Geometry.Parse(this.RawData);
                }

            }
        }

        public object GetGeometry()
        {
            return this._cachedGeometry;
        }
    }
}