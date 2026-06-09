using System.Globalization;

namespace TestTask.Utils
{
    public static class AmountParser
    {
        public static bool TryParse(string? value, out double result)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(value))
                return false;

            string normalized = value.Replace(',', '.');

            return double.TryParse(
                normalized,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out result);
        }

        public static double ParseOrDefault(string? value) =>
            TryParse(value, out double result) ? result : 0;

        public static string Format(double value) =>
            value.ToString("F2", CultureInfo.InvariantCulture);
    }
}
