using MvvmCharting.Common;

namespace DemoViewModel
{
    public class SomePoint : BindableBase
    {
        public override string ToString()
        {
            return $"SomePoint: t={this.t:F2}, Y={this.Y:F2}";
        }

        public double t { get; }
        public string tt=> $"{t}th";
        public double Y { get; }

        public SomePoint(double t, double y)
        {
            this.t = t;
            this.Y = y;
        }
    }
}