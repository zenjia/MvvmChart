using System;
using MvvmCharting.Common;
using MvvmCharting;

namespace Demo
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
    }
}