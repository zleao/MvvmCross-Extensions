using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            if (type != null)
                return (type.GetTypeInfo().IsGenericType && type.Name == "Nullable`1");

            return false;
        }

        /// <summary>
        /// Gets a collection of the methods declared and inherited by the current type.
        /// </summary>
        /// <param name="someType">Some type.</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetMethods(this Type someType)
        {
            var t = someType;
            while (t != null)
            {
                var ti = t.GetTypeInfo();
                foreach (var m in ti.DeclaredMethods)
                    yield return m;
                t = ti.BaseType;
            }
        }

        /// <summary>
        /// Gets a collection of the public properties declared by the current type.
        /// </summary>
        /// <param name="someType">Some type.</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties(this Type someType)
        {
            return someType.GetProperties(true);
        }

        /// <summary>
        /// Gets a collection of the properties declared by the current type.
        /// </summary>
        /// <param name="someType">Some type.</param>
        /// <param name="onlyPublic">if set to <c>true</c> [only public].</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties(this Type someType, bool onlyPublic)
        {
            var ti = someType.GetTypeInfo();
            var props = onlyPublic ? ti.DeclaredProperties.Where(p => (p.GetMethod != null && p.GetMethod.IsPublic) || (p.SetMethod != null && p.SetMethod.IsPublic)) : ti.DeclaredProperties;

            return props;
        }

        /// <summary>
        /// Gets a collection of the properties declared by the current type.
        /// </summary>
        /// <param name="someType">Some type.</param>
        /// <param name="onlyPublic">if set to <c>true</c> [only public].</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties(this Type someType, bool onlyPublic, bool includeInherited)
        {
            if (includeInherited)
                return someType.GetAllProperties(onlyPublic);

            return someType.GetProperties(onlyPublic);
        }


        /// <summary>
        /// Gets all properties of a type (including the inherited ones.
        /// </summary>
        /// <param name="someType">Some type.</param>
        /// <param name="onlyPublic">if set to <c>true</c> [only public].</param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetAllProperties(this Type someType, bool onlyPublic)
        {
            var t = someType;
            while (t != null)
            {
                var props = t.GetProperties(onlyPublic);

                foreach (var m in props)
                    yield return m;
                t = t.GetTypeInfo().BaseType;
            }
        }


        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <param name="someType">Some type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static object GetPropertyValue(this Type someType, string propertyName)
        {
            if (someType != null)
                return someType.GetTypeInfo().GetPropertyValue(propertyName);

            return null;
        }

        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <param name="someType">Some type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public static void SetPropertyValue(this Type someType, string propertyName, object value)
        {
            if (someType != null)
                someType.GetTypeInfo().SetPropertyValue(propertyName, value);
        }

        /// <summary>
        /// Returns a value that indicates whether the specified type can be assigned to the current type.
        /// </summary>
        /// <param name="someType">Some type.</param>
        /// <param name="baseType">Type of the base.</param>
        /// <returns></returns>
        public static bool IsAssignableFrom(this Type someType, Type baseType)
        {
            if (someType != null && baseType != null)
                return someType.GetTypeInfo().IsAssignableFrom(baseType.GetTypeInfo());

            return false;
        }
    }
}
