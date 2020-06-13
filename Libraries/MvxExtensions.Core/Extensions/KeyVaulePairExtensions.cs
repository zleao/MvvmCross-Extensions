using System.Collections.Generic;

namespace MvxExtensions.Core.Extensions
{
    /// <summary>
    /// Extensions for KeyValuePair type
    /// </summary>
    public static class KeyVaulePairExtensions
    {
        /// <summary>
        /// Determines if KeyValuePair{string, string} is null or if the key is null or empty.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   <c>true</c> if KeyValuePair{string, string} is null or if the key is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this KeyValuePair<string, string>? source)
        {
            return source?.Key.IsNullOrEmpty() != false;
        }
    }
}
