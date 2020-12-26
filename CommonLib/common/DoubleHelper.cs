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

        public static bool IsZero(this double value)
        {
            return value == 0;
        }

        public static bool IsInvalid(this double value)
        {
            return value.IsNaN() || double.IsInfinity(value);
        }

        public static bool NearlyEqual(this double value1, double value2, double tolerance = double.Epsilon)
        {
            if (value1.IsNaN() && value2.IsNaN())
            {
                return true;
            }
            return Math.Abs(value1 - value2) < tolerance;
        }

    }
}