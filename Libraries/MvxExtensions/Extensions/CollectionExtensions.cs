using System;
using System.Collections.Generic;
using System.Linq;

namespace MvxExtensions.Extensions
{
    /// <summary>
    /// Extensions for ICollection type
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Determines if the collection is null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   <c>true</c> if the collection is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count == 0;
        }

        /// <summary>
        /// Adds the range of items.
        /// Deals with null collections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items.</param>
        public static void SafeAddRange<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            if (source == null || items == null)
                return;

            try
            {
                if (items.Any())
                {
                    foreach (var item in items)
                    {
                        source.Add(item);
                    }
                }
            }
            catch (Exception)
            {
                //TODO: Log this error
            }
        }

        /// <summary>
        /// Adds the missing items to the collection.
        /// Deals with null collections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items.</param>
        public static void SafeAddMissing<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            SafeAddMissing(source, items, ShouldAddItem);
        }

        /// <summary>
        /// Adds the missing items to the collection.
        /// Deals with null collections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items.</param>
        /// <param name="validationFunc">The validation function.</param>
        /// <remarks>
        /// The missing item is only added if the validation func returns true
        /// </remarks>
        public static void SafeAddMissing<T>(this ICollection<T> source, IEnumerable<T> items, Func<T, bool> validationFunc)
        {
            if (source == null || items == null || validationFunc == null)
                return;

            try
            {
                if (items.Any())
                {
                    foreach (T item in items)
                    {
                        if (!source.Contains(item) && validationFunc.Invoke(item))
                            source.Add(item);
                    }
                }
            }
            catch (Exception)
            {
                //TODO: Log this error
            }
        }

        /// <summary>
        /// Default validation Func used in the AddMissing methods
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool ShouldAddItem<T>(T item)
        {
            return true;
        }

        /// <summary>
        /// Returns the element count of the collection.
        /// If collection is null or empty, returns zero
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static int SafeCount<T>(this ICollection<T> source)
        {
            return source.IsNullOrEmpty() ? 0 : source.Count;
        }
    }
}
