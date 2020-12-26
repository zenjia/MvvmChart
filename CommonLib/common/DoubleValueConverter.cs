using System;
using System.Globalization;

namespace MvvmChart.Common
{
    /// <summary>
    /// Convert object to double or vice versa
    /// 
    /// </summary>
    public static class DoubleValueConverter
    {
        public static double ObjectToDouble(object obj)
        {
            double returnValue;

            if (obj is DateTime)
            {
                returnValue = Convert.ToDouble(((DateTime)obj).Ticks);
            }
            else if (obj is DateTimeOffset)
            {
                returnValue = Convert.ToDouble(((DateTimeOffset)obj).Ticks);
            }
            else if (obj is IConvertible)
            {
                returnValue = ((IConvertible)obj).ToDouble(CultureInfo.CurrentCulture);
            }
            else
            {
                throw new MvvmChartException($"Type {obj?.GetType()} is not supported! Only type which implement IConvertible is supported!");
            }

            return returnValue;
        }

        public static DateTime DoubleToDateTime(double value)
        {
            return new DateTime(Convert.ToInt64(value)); 
        }

        public static DateTimeOffset DoubleToDateTimeOffset(double value)
        {
            return new DateTimeOffset(new DateTime(Convert.ToInt64(value)));  
        }

        public static object DoubleToObject(double value, Type type)
        {
            object obj;

            if (type == typeof(DateTime))
            {
                obj = new DateTime(Convert.ToInt64(value));
            }
            else if (type == typeof(DateTimeOffset))
            {
                obj = new DateTimeOffset(new DateTime(Convert.ToInt64(value)));
            }
            else
            {
                obj = Convert.ChangeType(value, type);
            }

            return obj;
        }
    }
}