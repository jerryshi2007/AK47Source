using System.Globalization;

namespace CheckCompileStatus
{
    public static class StringHelper
    {
        public static string Format<T>(this T obj, string formatValue)
        {
            return string.Format(CultureInfo.InvariantCulture, formatValue, obj);
        }
    }
}
