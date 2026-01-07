namespace UtilityKit.Extensions.String
{
    /// <summary>
    /// Parsing and conversion string extension methods.
    /// </summary>
    /// <remarks>
    /// Example usage:
    /// <code language="csharp">
    /// var i = "123".ParseIntOrDefault(); // 123
    /// var bad = "abc".ParseIntOrDefault(-1); // -1
    /// if ("2025-12-15".TryParseDateTime(out var dt)) Console.WriteLine(dt);
    /// var unix = "2025-12-15".ToUnixTimestamp();
    /// Guid.TryToGuid("...", out var g);
    /// var uri = "https://example.com".ToUri();
    /// </code>
    /// </remarks>
    public static class ParsingExtensions
    {
        /// <summary>
        /// Parses an int or returns <paramref name="defaultValue"/>.
        /// </summary>
        public static int ParseIntOrDefault(this string value, int defaultValue = 0)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var r) ? r : defaultValue;
        }

        /// <summary>
        /// Parses a long or returns <paramref name="defaultValue"/>.
        /// </summary>
        public static long ParseLongOrDefault(this string value, long defaultValue = 0L)
        {
            return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var r) ? r : defaultValue;
        }

        /// <summary>
        /// Parses a decimal or returns <paramref name="defaultValue"/>.
        /// </summary>
        public static decimal ParseDecimalOrDefault(this string value, decimal defaultValue = 0m, CultureInfo? culture = null)
        {
            var ci = culture ?? CultureInfo.InvariantCulture;
            return decimal.TryParse(value, NumberStyles.Number, ci, out var r) ? r : defaultValue;
        }

        /// <summary>
        /// Tries to parse a DateTime using an optional exact <paramref name="format"/>.
        /// </summary>
        public static bool TryParseDateTime(this string value, out DateTime result, string? format = null, CultureInfo? culture = null)
        {
            var ci = culture ?? CultureInfo.InvariantCulture;
            if (!string.IsNullOrEmpty(format))
            {
                return DateTime.TryParseExact(value, format, ci, DateTimeStyles.None, out result);
            }

            return DateTime.TryParse(value, ci, DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal, out result);
        }

        /// <summary>
        /// Parses a date string and returns Unix timestamp seconds or null on failure.
        /// </summary>
        public static long? ToUnixTimestamp(this string value, string? format = null, CultureInfo? culture = null)
        {
            if (!value.TryParseDateTime(out var dt, format, culture)) return null;
            return new DateTimeOffset(dt).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Tries to parse the string as a <see cref="Guid"/>.
        /// </summary>
        public static bool TryToGuid(this string value, out Guid result) =>
            Guid.TryParse(value, out result);

        /// <summary>
        /// Tries to create a <see cref="Uri"/> from the string; returns null on failure.
        /// </summary>
        public static Uri? ToUri(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri) ? uri : null;
        }
    }
}