using System;
using MvvmCharting.Common;

namespace MvvmCharting.Drawing
{
    public static class PointNSHelper
    {
        public static bool IsEmpty(this PointNS pt)
        {
            return pt.X.IsNaN() || pt.Y.IsNaN();
        }
        public static bool IsInvalid(this PointNS pt)
        {
            return pt.X.IsInvalid() || pt.Y.IsInvalid();
        }

    }
}