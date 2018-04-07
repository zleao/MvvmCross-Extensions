using Newtonsoft.Json;

namespace MvxExtensions.Libraries.Portable.Core.Extensions
{
    /// <summary>
    /// Extensions for any kind of object
    /// </summary>
    public static class AbstractExtensions
    {
        /// <summary>
        /// Creates a clone using json.net
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static T JSonClone<T>(this T source)
        {
            if (source == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }

        /// <summary>
        /// Converts an object to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static T JSonConvert<T>(this object source)
        {
            if (source == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }
    }
}
