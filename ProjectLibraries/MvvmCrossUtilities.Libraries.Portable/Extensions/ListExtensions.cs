using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Finds the zero based index of an item, using the search predicate.
        /// If item not found, returns -1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="searchPredicate">The search predicate.</param>
        /// <returns></returns>
        public static int FindIndex<T>(this IList<T> source, Func<T, bool> searchPredicate)
        {
            var item = source.FirstOrDefault(searchPredicate);
            if (item != null)
                return source.IndexOf(item);

            return -1;
        }

        /// <summary>
        /// Applies the action to every item in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action.</param>
        public static IList<T> ForEach<T>(this IList<T> source, Action<T> action)
        {
            if (source != null && source.Count > 0)
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
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filterPredicate">The filter predicate.</param>
        /// <returns></returns>
        public static bool RemoveAll<T>(this IList<T> source, Func<T, bool> filterPredicate)
        {
            try
            {
                var toRemove = source.Where(filterPredicate).ToList();
                toRemove.ForEach(r =>
                {
                    if (source.Contains(r))
                        source.Remove(r);
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the last item of a IList.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static bool RemoveLast<T>(this IList<T> source)
        {
            try
            {
                if (source != null && source.Count > 0)
                {
                    source.RemoveAt(source.Count - 1);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Adds an item to list if it is not allready in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="itemToAdd">The item to add.</param>
        public static void AddMissing(this IList source, object itemToAdd)
        {
            if (source == null)
                return;

            if (!source.Contains(itemToAdd))
                source.Add(itemToAdd);
        }
    }
}
