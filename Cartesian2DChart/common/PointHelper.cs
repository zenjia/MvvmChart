using System;
using System.Windows;
using MvvmChart.Common;

namespace MvvmCharting
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

    }
}