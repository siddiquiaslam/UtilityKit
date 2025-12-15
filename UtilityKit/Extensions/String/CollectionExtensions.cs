namespace UtilityKit.Extensions.String
{
    /// <summary>
    /// Collection and templating helpers for strings.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns an alternative when the string is null or empty.
        /// </summary>
        public static string OrDefault(this string? value, string defaultValue) =>
            string.IsNullOrEmpty(value) ? defaultValue : value!;

        /// <summary>
        /// Splits lines using CR/LF variations.
        /// </summary>
        public static string[] SplitLines(this string value)
        {
            if (value is null) return Array.Empty<string>();
            return Regex.Split(value, @"\r\n|\r|\n");
        }

        /// <summary>
        /// Returns the first line of the string.
        /// </summary>
        public static string FirstLine(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var idx = value.IndexOfAny(new[] { '\r', '\n' });
            return idx < 0 ? value : value.Substring(0, idx);
        }

        /// <summary>
        /// Returns the last line of the string.
        /// </summary>
        public static string LastLine(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var lines = value.SplitLines();
            return lines.Length == 0 ? string.Empty : lines[^1];
        }

        /// <summary>
        /// Tokenizes the string into words; optional punctuation removal.
        /// </summary>
        public static IEnumerable<string> GetWords(this string value, bool removePunctuation = true)
        {
            if (string.IsNullOrWhiteSpace(value)) return Array.Empty<string>();
            var text = removePunctuation ? Regex.Replace(value, @"[^\p{L}\p{Nd}\s'-]", "") : value;
            var matches = Regex.Matches(text, @"\b[\p{L}\p{Nd}'-]+\b");
            return matches.Select(m => m.Value);
        }

        /// <summary>
        /// Simple token interpolation using {key} placeholders.
        /// </summary>
        public static string Interpolate(this string template, IDictionary<string, string> values, bool throwOnMissing = false)
        {
            if (string.IsNullOrEmpty(template) || values == null || values.Count == 0) return template ?? string.Empty;
            return Regex.Replace(template, @"\{(?<key>[^\}]+)\}", match =>
            {
                var key = match.Groups["key"].Value;
                if (values.TryGetValue(key, out var v)) return v;
                if (throwOnMissing) throw new KeyNotFoundException($"Key '{key}' not found for interpolation.");
                return match.Value;
            });
        }
    }
}