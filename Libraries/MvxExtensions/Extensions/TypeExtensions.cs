using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvxExtensions.Extensions
{
    /// <summary>
    /// Extensions for Type
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="source">The type.</param>
        /// <returns></returns>
        public static bool IsNullable(this Type source)
        {
            if (source != null)
                return (source.GetTypeInfo().IsGenericType && source.Name == "Nullable`1");

            return false;
        }

        /// <summary>
        /// Gets a collection of the public properties declared by the current type.
        /// </summary>
        /// <param name="source">Some type.</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties(this Type source)
        {
            return source.GetProperties(true);
        }

        /// <summary>
        /// Gets a collection of the properties declared by the current type.
        /// </summary>
        /// <param name="source">Some type.</param>
        /// <param name="onlyPublic">if set to <c>true</c> [only public].</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties(this Type source, bool onlyPublic)
        {
            var ti = source.GetTypeInfo();
            var props = onlyPublic ? ti.DeclaredProperties.Where(p => (p.GetMethod != null && p.GetMethod.IsPublic) || (p.SetMethod != null && p.SetMethod.IsPublic)) : ti.DeclaredProperties;

            return props;
        }

        /// <summary>
        /// Gets a collection of the properties declared by the current type.
        /// </summary>
        /// <param name="source">Some type.</param>
        /// <param name="onlyPublic">if set to <c>true</c> [only public].</param>
        /// <param name="includeInherited">if set to <c>true</c> [include inherited].</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties(this Type source, bool onlyPublic, bool includeInherited)
        {
            if (includeInherited)
                return source.GetAllProperties(onlyPublic);

            return source.GetProperties(onlyPublic);
        }


        /// <summary>
        /// Gets all properties of a type (including the inherited ones.
        /// </summary>
        /// <param name="source">Some type.</param>
        /// <param name="onlyPublic">if set to <c>true</c> [only public].</param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetAllProperties(this Type source, bool onlyPublic)
        {
            var t = source;
            while (t != null)
            {
                var props = t.GetProperties(onlyPublic);

                foreach (var m in props)
                    yield return m;
                t = t.GetTypeInfo().BaseType;
            }
        }
    }
}
