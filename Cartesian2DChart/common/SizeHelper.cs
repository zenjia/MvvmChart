using System.Windows;

namespace MvvmCharting
{
    public static class SizeHelper
    {
        public static bool IsInvalid(this Size size)
        {
            return size.IsEmpty || (size.Width + size.Height).IsNaNOrZero();
        }
    }
}