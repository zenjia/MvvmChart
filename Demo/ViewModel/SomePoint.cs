using MvvmCharting;

namespace Demo
{
    public class SomePoint : BindableBase
    {
        public double t { get; }
        public double Y { get; }

        public SomePoint(double t, double y)
        {
            this.t = t;
            this.Y = y;
        }
    }
}