namespace UtilityKit.Extensions.String
{
    /// <summary>
    /// Encoding/decoding string extension methods.
    /// </summary>
    /// <remarks>
    /// Example usage:
    /// <code language="csharp">
    /// var b = "Hello".ToBase64();
    /// var t = b.FromBase64();
    /// var hx = "abc".ToHex();
    /// </code>
    /// </remarks>
    public static class EncodingExtensions
    {
        /// <summary>
        /// Encodes the string to Base64 using the provided encoding.
        /// </summary>
        public static string ToBase64(this string value, Encoding? encoding = null)
        {
            if (value is null) return string.Empty;
            var enc = encoding ?? Encoding.UTF8;
            return Convert.ToBase64String(enc.GetBytes(value));
        }

        /// <summary>
        /// Decodes a Base64 string to text using the provided encoding. Returns null on failure.
        /// </summary>
        public static string? FromBase64(this string value, Encoding? encoding = null)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            try
            {
                var bytes = Convert.FromBase64String(value);
                var enc = encoding ?? Encoding.UTF8;
                return enc.GetString(bytes);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the string to its hexadecimal representation using the provided encoding.
        /// </summary>
        public static string ToHex(this string value, Encoding? encoding = null)
        {
            if (value is null) return string.Empty;
            var enc = encoding ?? Encoding.UTF8;
            var bytes = enc.GetBytes(value);
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Converts a hexadecimal string into text using the provided encoding. Returns null on failure.
        /// </summary>
        public static string? FromHex(this string hex, Encoding? encoding = null)
        {
            if (string.IsNullOrWhiteSpace(hex)) return null;
            try
            {
                var cleaned = Regex.Replace(hex, @"\s+", "");
                if (cleaned.Length % 2 != 0) return null;
                var bytes = new byte[cleaned.Length / 2];
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(cleaned.Substring(i * 2, 2), 16);
                }

                var enc = encoding ?? Encoding.UTF8;
                return enc.GetString(bytes);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// HTML-encodes the string.
        /// </summary>
        public static string HtmlEncode(this string value) => string.IsNullOrEmpty(value) ? value : WebUtility.HtmlEncode(value);

        /// <summary>
        /// HTML-decodes the string.
        /// </summary>
        public static string HtmlDecode(this string value) => string.IsNullOrEmpty(value) ? value : WebUtility.HtmlDecode(value);

        /// <summary>
        /// Removes HTML tags from the string.
        /// </summary>
        public static string StripHtml(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return Regex.Replace(value, "<.*?>", string.Empty);
        }
    }
}