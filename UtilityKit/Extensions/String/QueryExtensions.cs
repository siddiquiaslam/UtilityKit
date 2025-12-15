namespace UtilityKit.Extensions.String
{
    /// <summary>
    /// URL/query and related helpers.
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Appends or replaces a query parameter in the provided URL.
        /// </summary>
        public static string AppendQueryParameter(this string url, string key, string value)
        {
            if (string.IsNullOrEmpty(url)) return $"{url}?{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(value)}";
            var parts = url.Split(new[] { '?' }, 2);
            var baseUrl = parts[0];
            var query = parts.Length > 1 ? parts[1] : string.Empty;
            var pairs = query.Length == 0 ? new List<string>() : query.Split('&').Where(p => !string.IsNullOrEmpty(p)).ToList();

            var encodedKey = WebUtility.UrlEncode(key);
            var replaced = false;
            for (int i = 0; i < pairs.Count; i++)
            {
                var p = pairs[i];
                var idx = p.IndexOf('=');
                var k = idx >= 0 ? p.Substring(0, idx) : p;
                if (string.Equals(k, encodedKey, StringComparison.OrdinalIgnoreCase))
                {
                    pairs[i] = $"{encodedKey}={WebUtility.UrlEncode(value)}";
                    replaced = true;
                    break;
                }
            }

            if (!replaced)
            {
                pairs.Add($"{encodedKey}={WebUtility.UrlEncode(value)}");
            }

            var newQuery = string.Join("&", pairs);
            return string.IsNullOrEmpty(newQuery) ? baseUrl : baseUrl + "?" + newQuery;
        }

        /// <summary>
        /// Removes a query parameter from the URL.
        /// </summary>
        public static string RemoveQueryParameter(this string url, string key)
        {
            if (string.IsNullOrEmpty(url)) return url ?? string.Empty;
            var parts = url.Split(new[] { '?' }, 2);
            if (parts.Length == 1) return url;
            var baseUrl = parts[0];
            var query = parts[1];
            var encodedKey = WebUtility.UrlEncode(key);
            var pairs = query.Split('&')
                .Where(p =>
                {
                    var idx = p.IndexOf('=');
                    var k = idx >= 0 ? p.Substring(0, idx) : p;
                    return !string.Equals(k, encodedKey, StringComparison.OrdinalIgnoreCase);
                })
                .Where(p => !string.IsNullOrEmpty(p))
                .ToArray();

            return pairs.Length == 0 ? baseUrl : baseUrl + "?" + string.Join("&", pairs);
        }

        /// <summary>
        /// Retrieves a query parameter value from the URL or null.
        /// </summary>
        public static string? GetQueryParameter(this string url, string key)
        {
            if (string.IsNullOrEmpty(url)) return null;
            var parts = url.Split(new[] { '?' }, 2);
            if (parts.Length == 1) return null;
            var query = parts[1];
            var encodedKey = WebUtility.UrlEncode(key);
            foreach (var p in query.Split('&'))
            {
                if (string.IsNullOrEmpty(p)) continue;
                var idx = p.IndexOf('=');
                var k = idx >= 0 ? p.Substring(0, idx) : p;
                var v = idx >= 0 ? p.Substring(idx + 1) : string.Empty;
                if (string.Equals(k, encodedKey, StringComparison.OrdinalIgnoreCase))
                {
                    return WebUtility.UrlDecode(v);
                }
            }
            return null;
        }

        /// <summary>
        /// Extracts the MIME type from a data URI (e.g. data:image/png;base64,...).
        /// </summary>
        public static string? GetMimeTypeFromDataUri(this string dataUri)
        {
            if (string.IsNullOrWhiteSpace(dataUri)) return null;
            var m = Regex.Match(dataUri, @"^data:(?<type>[^;]+);base64,");
            if (!m.Success) return null;
            return m.Groups["type"].Value;
        }

        /// <summary>
        /// Converts a numeric-byte value string into a human-readable size (e.g. "1.0 KB").
        /// </summary>
        public static string HumanizeByteSize(this string numericBytes, int decimals = 1)
        {
            if (string.IsNullOrWhiteSpace(numericBytes)) return "0 B";
            if (!double.TryParse(numericBytes, NumberStyles.Any, CultureInfo.InvariantCulture, out var bytes)) return numericBytes;
            var sizes = new[] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            var order = 0;
            while (bytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                bytes /= 1024;
            }

            return $"{Math.Round(bytes, decimals)} {sizes[order]}";
        }
    }
}