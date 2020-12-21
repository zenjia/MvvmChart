using System.Windows;
using System.Windows.Media;

namespace MvvmCharting
{
    /// <summary>
    /// A handy class to draw step poly line series.
    /// </summary>
    public class StepLineSeries : PathSeries
    {
        public StepLineSeries()
        {
            this.GeometryBuilder = new StepLineGeometryBuilder();
        }
 

  
    }
}