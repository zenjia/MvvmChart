using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{
    public class StepLineSeries : LineSeries
    {
        protected override PointCollection GetPointCollection()
        {
    
            PointCollection pc = new PointCollection();

 
            foreach (var item in this.DataPointViewModels)
            {
                var pt = item.Position;
                if (pc.Count > 0)
                {
                    pc.Add(new Point(pt.X, pc[pc.Count - 1].Y));
                }
                pc.Add(pt);

               ;
            }

            return pc;
        }
    }
}