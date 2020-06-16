using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MvxExtensions.Core.Extensions
{
    /// <summary>
    /// Extensions for IList type
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Finds the zero based index of an item, using the search predicate.
        /// If item not found, returns -1
        /// Deals with null IList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="searchPredicate">The search predicate.</param>
        /// <returns></returns>
        public static int SafeFindIndex<T>(this IList<T> source, Func<T, bool> searchPredicate)
        {
            var item = source.FirstOrDefault(searchPredicate);
            if (item != null)
                return source.IndexOf(item);

            return -1;
        }

        /// <summary>
        /// Applies the action to every item in the list
        /// Deals with null IList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action.</param>
        public static IList<T> SafeForEach<T>(this IList<T> source, Action<T> action)
        {
            if (source?.Count > 0)
            {
                foreach (var item in source)
                {
                    action(item);
                }
            }

            return source;
        }

        /// <summary>
        /// Removes all the items filtered by a predicate.
        /// Deals with null IList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filterPredicate">The filter predicate.</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
        /// <returns></returns>
        public static bool SafeRemoveAll<T>(this IList<T> source, Func<T, bool> filterPredicate, bool throwException = false)
        {
            try
            {
                var toRemove = source.Where(filterPredicate).ToList();
                toRemove.SafeForEach(r =>
                {
                    if (source.Contains(r))
                        source.Remove(r);
                });

                return true;
            }
            catch (Exception) when (!throwException)
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the last item of a IList.
        /// Deals with null IList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
        /// <returns></returns>
        public static bool SafeRemoveLast<T>(this IList<T> source, bool throwException = false)
        {
            try
            {
                if (source?.Count > 0)
                {
                    source.RemoveAt(source.Count - 1);
                }

                return true;
            }
            catch (Exception) when (!throwException)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the System.Collections.Generic.IList.
        /// Deals with null IList
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">The System.Collections.Generic.IList to check for index</param>
        /// <param name="value">The value to check with</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public static int SafeIndexOf<T>(this IList<T> source, T value)
        {
            return source?.IndexOf(value) ?? -1;
        }

        /// <summary>
        /// Checks if the value is present in the source list
        /// Deals with null IList
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">The source list to iterate</param>
        /// <param name="value">The object to locate in the source</param>
        /// <returns>True if item is found; otherwhise false</returns>
        public static bool SafeContains<T>(this IList<T> source, T value)
        {
            if (source != null)
                return source.Contains(value);

            return false;
        }

        /// <summary>
        /// Adds an item to list if it is not allready in the list.
        /// Deals with null IList
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="itemToAdd">The item to add.</param>
        public static void SafeAddMissing(this IList source, object itemToAdd)
        {
            if (source == null)
                return;

            if (!source.Contains(itemToAdd))
                source.Add(itemToAdd);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the System.Collections.IList.
        /// Deals with null IList
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="valueToRemove">The value to remove.</param>
        public static void SafeRemove(this IList source, object valueToRemove)
        {
            source?.Remove(valueToRemove);
        }

        /// <summary>
        /// Removes all items from the System.Collections.IList.
        /// Deals with null IList
        /// </summary>
        /// <param name="source">The source.</param>
        public static void SafeClear(this IList source)
        {
            source?.Clear();
        }

        /// <summary>
        /// Checks if the value is present in the source list
        /// Deals with null IList
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool SafeContains(this IList source, object value)
        {
            if (source != null)
                return source.Contains(value);

            return false;
        }

        /// <summary>
        /// Removes the item at the specified index
        /// Deals with null or empty IList
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        public static void SafeRemoveAt(this IList source, int index)
        {
            if (source != null && source.Count > index)
                source.RemoveAt(index);
        }
    }
}
