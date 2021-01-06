using System;
using System.Windows;
using MvvmCharting.Common;
 

namespace MvvmCharting.WpfFX
{
    public static class PointHelper
    {
        public static Point Empty = new Point(Double.NaN, double.NaN);

        public static bool IsEmpty(this Point pt)
        {
            return pt.X.IsNaN() || pt.Y.IsNaN();
        }
        public static bool IsInvalid(this Point pt)
        {
            return pt.X.IsInvalid() || pt.Y.IsInvalid();
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