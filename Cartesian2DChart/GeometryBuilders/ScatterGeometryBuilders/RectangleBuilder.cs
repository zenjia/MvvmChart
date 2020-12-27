using System.Windows;
using System.Windows.Media;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX
{
    public class RectangleBuilder : IScatterGeometryBuilder
    {
        public Size Size { get; set; }
        public object GetGeometry()
        {
            return new RectangleGeometry(new Rect(this.Size));
        }
    }
}