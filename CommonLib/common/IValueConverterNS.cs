using System.Globalization;

namespace MvvmCharting.Common
{
    /// <summary>
    /// This is the .NET Standard version of WPF/UWP IValueConverter interface.
    /// </summary>
    public interface IValueConverterNS
    {
        object ConverterTo(object value, CultureInfo culture);
    }
}
