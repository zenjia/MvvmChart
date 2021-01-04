using System.Windows;
using System.Windows.Media;
using MvvmCharting.Series;

namespace MvvmCharting.WpfFX.Series
{
    public class RectangleGeometryBuilder : IScatterGeometryBuilder
    {
        public Size Size { get; set; }
        public object GetGeometry()
        {
            return new RectangleGeometry(new Rect(this.Size));
        }
    }
}