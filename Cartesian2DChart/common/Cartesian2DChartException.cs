using System;

namespace MvvmCharting
{
    public class Cartesian2DChartException : Exception
    {
        public Cartesian2DChartException(string msg)
            :base(msg)
        {
            
        }

    }

    public class MvvmChartUnexpectedTypeException : Exception
    {
        public MvvmChartUnexpectedTypeException(string msg)
            : base(msg)
        {

        }

    }
}