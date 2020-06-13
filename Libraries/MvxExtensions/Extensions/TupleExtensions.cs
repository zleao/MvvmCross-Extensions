using MvxExtensions.Core.Extensions;
using System;

namespace MvxExtensions.Extensions
{
    /// <summary>
    /// Extensions for Tuple type
    /// </summary>
    public static class TupleExtensions
    {
        /// <summary>
        /// Determines whether is null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this Tuple<string, T> source)
        {
            return source == null || source.Item1.IsNullOrEmpty();
        }
    }
}
