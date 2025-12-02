using System.Collections.Generic;
using System.Linq;

namespace CMS_2026.Common
{
    public static class Extensions
    {
        /// <summary>
        /// Returns an empty enumerable if the source is null, otherwise returns the source.
        /// Equivalent to Root.EmptyIfNull() in everflorcom_new.
        /// </summary>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}

