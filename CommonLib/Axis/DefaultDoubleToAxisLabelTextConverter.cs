using System;
using System.Globalization;
using MvvmChart.Common;

namespace MvvmCharting.Axis
{
    /// <summary>
    /// Default double to string converter for AxisLabelText.
    /// </summary>
    public class DefaultDoubleToAxisLabelTextConverter : IValueConverterNS
    {
       

        public object ConverterTo(object value, CultureInfo culture)
        {
            return string.Format("{0:F1}", value);
        }
    }
}