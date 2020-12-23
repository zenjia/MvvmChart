using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{
    public class RectangleBuilder : IScatterGeometryBuilder
    {
        public Size Size { get; set; }
        public Geometry GetGeometry()
        {
            return new RectangleGeometry(new Rect(this.Size));
        }
    }
}