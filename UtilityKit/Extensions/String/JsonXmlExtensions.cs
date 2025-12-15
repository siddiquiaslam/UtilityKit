using System.Text.Json;
using System.Xml.Linq;

namespace UtilityKit.Extensions.String
{
    /// <summary>
    /// JSON/XML helper extension methods.
    /// </summary>
    public static class JsonXmlExtensions
    {
        /// <summary>
        /// Quick check whether the string is JSON (object or array).
        /// </summary>
        public static bool IsJson(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var s = value.Trim();
            if (!(s.StartsWith("{") || s.StartsWith("["))) return false;
            try
            {
                using var doc = JsonDocument.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Pretty-prints JSON (returns original on parse failure).
        /// </summary>
        public static string PrettyPrintJson(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            try
            {
                using var doc = JsonDocument.Parse(value);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var element = doc.RootElement.Clone();
                return JsonSerializer.Serialize(element, options);
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        /// Quick check whether the string is valid XML.
        /// </summary>
        public static bool IsXml(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            try
            {
                XDocument.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Pretty-prints XML (returns original on parse failure).
        /// </summary>
        public static string PrettyPrintXml(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            try
            {
                var doc = XDocument.Parse(value);
                return doc.ToString();
            }
            catch
            {
                return value;
            }
        }
    }
}