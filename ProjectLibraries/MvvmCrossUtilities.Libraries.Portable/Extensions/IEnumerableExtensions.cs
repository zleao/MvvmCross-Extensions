using System.Collections;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns the element count of the IEnumerable.
        /// If collection null or empty, returns zero
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static int CountOrZero(this IEnumerable source)
        {
            if (source == null)
                return 0;

            var count = 0;
            foreach (var item in source)
                count++;

            return count;
        }
    }
}
