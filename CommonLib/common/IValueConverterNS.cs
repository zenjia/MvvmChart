using System.Globalization;

namespace MvvmChart.Common
{
    public interface IValueConverterNS
    {
        object ConverterTo(object value, CultureInfo culture);
    }
}
