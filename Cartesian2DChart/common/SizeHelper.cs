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
    }
}