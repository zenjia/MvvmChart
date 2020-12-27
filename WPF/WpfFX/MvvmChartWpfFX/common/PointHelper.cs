using System;
using System.Windows;
using MvvmCharting.Common;
using MvvmCharting.Drawing;

namespace MvvmCharting.WpfFX
{
    public static class PointHelper
    {
        public static Point EmptyPoint = new Point(Double.NaN, double.NaN);

        public static bool IsEmpty(this Point pt)
        {
            return pt.X.IsNaN() || pt.Y.IsNaN();
        }
        public static bool IsInvalid(this Point pt)
        {
            return pt.X.IsInvalid() || pt.Y.IsInvalid();
        }

        public static Point ToPoint(this PointNS source)
        {
            return new Point(source.X, source.Y);
        }

    }
}