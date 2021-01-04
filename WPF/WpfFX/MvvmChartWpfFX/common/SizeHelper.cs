using System.Windows;
using MvvmCharting.Common;

namespace MvvmCharting.WpfFX
{
    public static class SizeHelper
    {
        public static bool IsInvalid(this Size size)
        {
            return size.IsEmpty || (size.Width + size.Height).IsNaNOrZero();
        }

        public static bool NearlyEqual(this Size size, Size other)
        {
            if (size.IsEmpty && other.IsEmpty)
            {
                return true;
            }

            return size.Width.NearlyEqual(other.Width, 0.5) && size.Height.NearlyEqual(other.Height, 0.5);
        }
    }
}