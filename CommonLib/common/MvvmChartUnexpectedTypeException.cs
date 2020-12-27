using System;

namespace MvvmCharting.Common
{
    public class MvvmChartUnexpectedTypeException : Exception
    {
        public MvvmChartUnexpectedTypeException(string msg)
            : base(msg)
        {

        }

    }
}