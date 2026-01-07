namespace UtilityKit.Extensions.String
{
    /// <summary>
    /// Similarity and distance algorithms for strings.
    /// </summary>
    /// <remarks>
    /// Example usage:
    /// <code language="csharp">
    /// var d = "kitten".LevenshteinDistance("sitting");
    /// var score = "hello".SimilarityScore("hallo");
    /// </code>
    /// </remarks>
    public static class SimilarityExtensions
    {
        /// <summary>
        /// Computes the Levenshtein edit distance between two strings.
        /// </summary>
        public static int LevenshteinDistance(this string s, string t)
        {
            if (s == null) s = string.Empty;
            if (t == null) t = string.Empty;
            var n = s.Length;
            var m = t.Length;
            if (n == 0) return m;
            if (m == 0) return n;

            var d = new int[n + 1, m + 1];

            for (var i = 0; i <= n; d[i, 0] = i++) { }
            for (var j = 0; j <= m; d[0, j] = j++) { }

            for (var i = 1; i <= n; i++)
            {
                for (var j = 1; j <= m; j++)
                {
                    var cost = t[j - 1] == s[i - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        /// <summary>
        /// Returns a normalized similarity score in [0..1] based on Levenshtein distance.
        /// </summary>
        public static double SimilarityScore(this string s, string other)
        {
            if (s is null && other is null) return 1.0;
            if (s is null || other is null) return 0.0;
            var dist = s.LevenshteinDistance(other);
            var max = Math.Max(s.Length, other.Length);
            if (max == 0) return 1.0;
            return 1.0 - (double)dist / max;
        }
    }
}