namespace UtilityKit.Extensions.String
{
    /// <summary>
    /// Formatting and transformation string extension methods.
    /// </summary>
    /// <remarks>
    /// Example usage:
    /// <code language="csharp">
    /// var title = "hello world".ToTitleCase();
    /// var slug = "Café au lait".ToSlug();
    /// var mask = "1234567890".Mask(2,2);
    /// var file = "a/b\n.txt".SafeFileName();
    /// </code>
    /// </remarks>
    public static class FormattingExtensions
    {
        /// <summary>
        /// Converts string to title case using the provided or current culture.
        /// </summary>
        public static string ToTitleCase(this string value, CultureInfo? culture = null)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var ci = culture ?? CultureInfo.CurrentCulture;
            return ci.TextInfo.ToTitleCase(value.ToLower(ci));
        }

        /// <summary>
        /// Produces a URL-safe slug from the input string.
        /// </summary>
        public static string ToSlug(this string value, bool removeAccents = true)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            var s = value.Trim();
            if (removeAccents)
            {
                s = s.RemoveDiacritics();
            }

            s = s.ToLowerInvariant();
            s = Regex.Replace(s, @"[^a-z0-9]+", "-");
            s = Regex.Replace(s, "-{2,}", "-").Trim('-');
            return s;
        }

        /// <summary>
        /// Returns a trimmed string or null when result is empty.
        /// </summary>
        public static string? TrimToNull(this string? value)
        {
            if (value is null) return null;
            var t = value.Trim();
            return t.Length == 0 ? null : t;
        }

        /// <summary>
        /// Ensures string ends with <paramref name="suffix"/> (no-op if suffix is null/empty).
        /// </summary>
        public static string EnsureEndsWith(this string value, string suffix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(suffix)) return value;
            return value.EndsWith(suffix, comparison) ? value : value + suffix;
        }

        /// <summary>
        /// Ensures string starts with <paramref name="prefix"/> (no-op if prefix is null/empty).
        /// </summary>
        public static string EnsureStartsWith(this string value, string prefix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(prefix)) return value;
            return value.StartsWith(prefix, comparison) ? value : prefix + value;
        }

        /// <summary>
        /// Truncates to <paramref name="maxLength"/> and appends optional <paramref name="suffix"/>.
        /// </summary>
        public static string Truncate(this string value, int maxLength, string? suffix = "...")
        {
            if (value is null) return string.Empty;
            if (maxLength <= 0) return string.Empty;
            if (value.Length <= maxLength) return value;
            var suffixToUse = suffix ?? string.Empty;
            var maxContent = Math.Max(0, maxLength - suffixToUse.Length);
            if (maxContent == 0) return suffixToUse;
            return value.Substring(0, maxContent) + suffixToUse;
        }

        /// <summary>
        /// Returns the leftmost <paramref name="length"/> characters, safe with bounds.
        /// </summary>
        public static string Left(this string value, int length)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (length <= 0) return string.Empty;
            return value.Length <= length ? value : value.Substring(0, length);
        }

        /// <summary>
        /// Returns the rightmost <paramref name="length"/> characters, safe with bounds.
        /// </summary>
        public static string Right(this string value, int length)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (length <= 0) return string.Empty;
            return value.Length <= length ? value : value.Substring(value.Length - length, length);
        }

        /// <summary>
        /// Reverses characters in the string.
        /// </summary>
        public static string Reverse(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var arr = value.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /// <summary>
        /// Normalizes Unicode string to the specified <paramref name="form"/>.
        /// </summary>
        public static string ToNormalized(this string value, NormalizationForm form = NormalizationForm.FormC) =>
            string.IsNullOrEmpty(value) ? value : value.Normalize(form);

        /// <summary>
        /// Removes diacritics (accents) from characters.
        /// </summary>
        public static string RemoveDiacritics(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var normalized = value.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Returns a safe file name by replacing invalid characters with <paramref name="replacement"/>.
        /// </summary>
        public static string SafeFileName(this string value, char replacement = '_')
        {
            if (string.IsNullOrEmpty(value)) return value;
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(value.Length);
            foreach (var ch in value)
            {
                sb.Append(invalid.Contains(ch) ? replacement : ch);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Masks a string leaving <paramref name="unmaskedStart"/> and <paramref name="unmaskedEnd"/> visible.
        /// </summary>
        public static string Mask(this string value, int unmaskedStart = 1, int unmaskedEnd = 1, char maskChar = '*')
        {
            if (string.IsNullOrEmpty(value)) return value;
            var len = value.Length;
            if (unmaskedStart + unmaskedEnd >= len) return new string(maskChar, len);
            var sb = new StringBuilder();
            sb.Append(value.Substring(0, unmaskedStart));
            sb.Append(new string(maskChar, len - unmaskedStart - unmaskedEnd));
            sb.Append(value.Substring(len - unmaskedEnd, unmaskedEnd));
            return sb.ToString();
        }

        /// <summary>
        /// Collapses consecutive whitespace to single spaces and trims.
        /// </summary>
        public static string NormalizeWhitespace(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return Regex.Replace(value, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Removes all whitespace characters.
        /// </summary>
        public static string RemoveWhitespace(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return Regex.Replace(value, @"\s+", string.Empty);
        }
    }
}