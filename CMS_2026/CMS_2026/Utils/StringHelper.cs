using System;
using System.Text.RegularExpressions;

namespace CMS_2026.Utils
{
    public static class StringHelper
    {
        public static string GetBefore(this string source, string value)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
                return source ?? string.Empty;

            int index = source.IndexOf(value, StringComparison.Ordinal);
            return index >= 0 ? source.Substring(0, index) : source;
        }

        public static string GetAfter(this string source, string value)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
                return string.Empty;

            int index = source.IndexOf(value, StringComparison.Ordinal);
            return index >= 0 && index + value.Length < source.Length 
                ? source.Substring(index + value.Length) 
                : string.Empty;
        }

        public static string GetBeforeLast(this string source, string value)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
                return source ?? string.Empty;

            int index = source.LastIndexOf(value, StringComparison.Ordinal);
            return index >= 0 ? source.Substring(0, index) : source;
        }

        public static string GetAfterLast(this string source, string value)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
                return string.Empty;

            int index = source.LastIndexOf(value, StringComparison.Ordinal);
            return index >= 0 && index + value.Length < source.Length 
                ? source.Substring(index + value.Length) 
                : string.Empty;
        }

        public static string? NullIfWhiteSpace(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value.Trim();
        }
    }
}

