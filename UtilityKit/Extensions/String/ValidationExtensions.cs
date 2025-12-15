using System.Net.Mail;

namespace UtilityKit.Extensions.String
{
    /// <summary>
    /// Validation-related string extension methods.
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Returns true when <paramref name="value"/> is a valid email address.
        /// Uses <see cref="MailAddress"/> for parsing and ensures the parsed address matches the input.
        /// </summary>
        public static bool IsValidEmail(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            try
            {
                var addr = new MailAddress(value);
                return string.Equals(addr.Address, value.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true when <paramref name="value"/> is a valid absolute HTTP/HTTPS URL.
        /// </summary>
        public static bool IsValidUrl(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                return false;
            }

            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }

        /// <summary>
        /// Returns true when <paramref name="value"/> contains digits only.
        /// </summary>
        public static bool IsDigitsOnly(this string value) =>
            !string.IsNullOrEmpty(value) && value.All(char.IsDigit);

        /// <summary>
        /// Returns true when <paramref name="value"/> contains alphabetic characters only.
        /// </summary>
        public static bool IsAlpha(this string value) =>
            !string.IsNullOrEmpty(value) && value.All(char.IsLetter);

        /// <summary>
        /// Returns true when <paramref name="value"/> contains letters or digits only.
        /// </summary>
        public static bool IsAlphaNumeric(this string value) =>
            !string.IsNullOrEmpty(value) && value.All(char.IsLetterOrDigit);

        /// <summary>
        /// Returns true when <paramref name="value"/> is a valid GUID.
        /// </summary>
        public static bool IsValidUuid(this string value) =>
            !string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out _);

        /// <summary>
        /// Returns true when <paramref name="value"/> is a palindrome. Options allow ignoring punctuation and case.
        /// </summary>
        public static bool IsPalindrome(this string value, bool ignorePunctuation = true, bool ignoreCase = true)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;

            var s = value;
            if (ignorePunctuation)
            {
                s = Regex.Replace(s, @"[^\p{L}\p{Nd}]+", string.Empty);
            }
            if (ignoreCase) s = s.ToLowerInvariant();

            int i = 0, j = s.Length - 1;
            while (i < j)
            {
                if (s[i] != s[j]) return false;
                i++; j--;
            }
            return true;
        }
    }
}