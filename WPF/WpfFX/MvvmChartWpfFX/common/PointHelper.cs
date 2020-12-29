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
        public static Point[] ToPoints(this PointNS[] sources)
        {
            var arr = new Point[sources.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = sources[i].ToPoint();
            }
            return arr;
        }

        public static Point[] ToPointsReversed(this PointNS[] sources)
        {
            var arr = new Point[sources.Length];
            int j = 0;
            for (int i = sources.Length - 1; i >= 0 ; i--)
            {
                arr[j++] = sources[i].ToPoint();
            }
            return arr;
        }

        public static Point[] Reversed(this Point[] sources)
        {
            
            var arr = new Point[sources.Length];
            int j = 0;
            for (int i = sources.Length - 1; i >= 0; i--)
            {
                arr[j++] = sources[i];
            }
            return arr;
        }
    }
}