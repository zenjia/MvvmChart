using System;

namespace MvvmChart.Common
{
    public class MvvmChartUnexpectedTypeException : Exception
    {
        public MvvmChartUnexpectedTypeException(string msg)
            : base(msg)
        {

        }

    }
}