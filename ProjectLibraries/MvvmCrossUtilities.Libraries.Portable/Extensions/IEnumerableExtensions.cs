using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    /// <summary>
    /// Extensions for IEnumerables type
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns the element count of the IEnumerable.
        /// If collection null or empty, returns zero
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static int SafeCount(this IEnumerable source)
        {
            if (source == null)
                return 0;

            var count = 0;
            foreach (var item in source)
                count++;

            return count;
        }

        /// <summary>
        /// Determines if an object is present in the enumeration.
        /// Deals with null IEnumerables
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="valueToFind">The value to find.</param>
        /// <returns></returns>
        public static bool SafeIsValueIn<T>(this IEnumerable<T> source, T valueToFind)
        {
            return source != null ? source.Contains(valueToFind) : false;
        }

        /// <summary>
        /// Returns the first element of a sequence, or a default value if the sequence
        /// contains no elements.
        /// Safe to use with null IEnumerables
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">The System.Collections.Generic.IEnumerable to return the first element of</param>
        /// <returns>default(T) if source is empty; otherwise, the first element in source.</returns>
        public static T SafeFirstOrDefault<T>(this IEnumerable<T> source)
        {
            return source != null ? source.FirstOrDefault() : default(T);
        }

        /// <summary>
        /// Returns the first element in a sequence that satisfies a specified condition.
        /// Safe to use with null IEnumerables
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">The System.Collections.Generic.IEnumerable to return the first element of</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// default(T) if source is empty; otherwise, The first element in the sequence 
        /// that passes the test in the specified predicate function.
        /// </returns>
        public static T SafeFirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source != null ? source.FirstOrDefault(predicate) : default(T);
        }

        /// <summary>
        /// Returns the last element of a sequence, or a default value if the sequence
        /// contains no elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable to return the last element of.</param>
        /// <returns>
        /// default(TSource) if the source sequence is empty; otherwise, the last element
        /// in the System.Collections.Generic.IEnumerable.
        /// </returns>
        public static T SafeLastOrDefault<T>(this IEnumerable<T> source)
        {
            return source != null ? source.LastOrDefault() : default(T);
        }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a condition or a default
        /// value if no such element is found.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable to return the last element of.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// default(T) if source is empty; otherwise, The last element in the sequence 
        /// that passes the test in the specified predicate function.
        /// </returns>
        public static T SafeLastOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source != null ? source.LastOrDefault(predicate) : default(T);
        }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// Safe to use with null IEnumerables
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">The System.Collections.Generic.IEnumerable to apply the predicate</param>
        /// <param name="predicate">The predicate.</param>
        public static bool SafeAny<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source != null ? source.Any(predicate) : false;
        }

        /// <summary>
        ///  Determines whether a sequence contains a specified element by using the default
        ///  equality comparer.
        ///  Safe to use with null IEnumerables
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">The System.Collections.Generic.IEnumerable to check for value</param>
        /// <param name="value">The value to check with</param>
        /// <returns></returns>
        public static bool SafeContains<T>(this IEnumerable<T> source, T value)
        {
            return source != null ? source.Contains(value) : false;
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the element's index.
        /// Safe to use with null IEnumerables
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
        /// <returns>An System.Collections.Generic.IEnumerable whose elements are the result of invoking the transform function on each element of source. Null if source is null</returns>
        public static IEnumerable<TResult> SafeSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            return source != null ? source.Select(selector) : null;
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// Safe to use with null IEnumerables
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An System.Collections.Generic.IEnumerable whose elements are the result of invoking the transform function on each element of source. Null is source is null</returns>
        public static IEnumerable<TResult> SafeSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source != null ? source.Select(selector) : null;
        }
    }
}
