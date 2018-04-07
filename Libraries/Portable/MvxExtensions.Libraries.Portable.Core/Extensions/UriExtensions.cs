using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MvxExtensions.Libraries.Portable.Core.Extensions
{
    /// <summary>
    /// Extensions for Uri type
    /// </summary>
    public static class UriExtensions
    {
        private static readonly Regex QueryStringRegex = new Regex(@"[\?&](?<name>[^&=]+)=(?<value>[^&=]+)");

        /// <summary>
        /// Parses the query string.
        /// </summary>
        /// <param name="source">The URI.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">uri</exception>
        public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(this Uri source)
        {
            if (source == null)
                throw new ArgumentException("uri");

            var matches = QueryStringRegex.Matches(source.OriginalString);
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                yield return new KeyValuePair<string, string>(match.Groups["name"].Value, match.Groups["value"].Value);
            }
        }
    }
}
