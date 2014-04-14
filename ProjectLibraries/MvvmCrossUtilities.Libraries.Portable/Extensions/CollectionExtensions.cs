using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
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
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <param name="items">The items.</param>
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            if (source == null || items == null)
                return;

            try
            {
                if (items != null && items.Count() > 0)
                {
                    foreach (T item in items)
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
        /// </summary>
        /// <param name="items">The items.</param>
        public static void AddMissing<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            AddMissing<T>(source, items, new Func<T, bool>(ShouldAddItem));            
        }

        /// <summary>
        /// Adds the missing items to the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="items">The items.</param>
        /// <param name="validationFunc">The validation function.</param>
        /// <remarks>
        /// The missing item is only added if the validation func returns true
        /// </remarks>
        public static void AddMissing<T>(this ICollection<T> source, IEnumerable<T> items, Func<T,bool> validationFunc)
        {
            if (source == null || items == null || validationFunc == null)
                return;

            try
            {
                if (items != null && items.Count() > 0)
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
        /// If collection null or empty, returns zero
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static int CountOrZero<T>(this ICollection<T> source)
        {
            return source.IsNullOrEmpty() ? 0 : source.Count;
        }
    }
}
