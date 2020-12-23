using System;

namespace MvvmChart.Common
{
    public static class DoubleHelper
    {
        public static bool IsNaN(this double value)
        {
            return double.IsNaN(value);
        }

        public static bool IsNaNOrZero(this double value)
        {
            return double.IsNaN(value) || value == 0;
        }

        public static bool NearlyEqual(this double value1, double value2, double tolerance = 0.0000001)
        {
            if (value1.IsNaN() && value2.IsNaN())
            {
                return true;
            }
            return Math.Abs(value1 - value2) < tolerance;
        }

    }
}