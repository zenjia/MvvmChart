using MvvmCharting;

namespace Demo
{
    public class SomePoint : BindableBase
    {
        public int t { get; }
        public int Y { get; }

        public SomePoint(int t, int y)
        {
            this.t = t;
            this.Y = y;
        }
    }
}