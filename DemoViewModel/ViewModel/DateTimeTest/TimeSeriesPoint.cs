using System;
using System.Globalization;
using MvvmCharting.Common;

namespace DemoViewModel
{
    public class TimeSeriesPoint : BindableBase
    {
        public DateTimeOffset t { get; }
        public double Y { get; }

        public TimeSeriesPoint(DateTimeOffset t, double y)
        {
            this.t = t;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"{t.ToString("yy-MMM-dd ddd",  CultureInfo.CurrentCulture)}: {Y:F2}";
        }
    }
}