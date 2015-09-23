using System;
using System.Reflection;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    /// <summary>
    /// Extensions for object type
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets the property value.
        /// Deals with null object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static object SafeGetPropertyValue(this object source, string propertyName)
        {
            if (source != null)
            {
                var prop = SafeGetPropertyInfoRecursively(source.GetType(), propertyName);
                if (prop != null)
                    return prop.GetValue(source);
            }

            return null;
        }

        /// <summary>
        /// Sets the property value.
        /// Deals with null object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public static void SafeSetPropertyValue(this object source, string propertyName, object value)
        {
            if (source != null)
            {
                var prop = SafeGetPropertyInfoRecursively(source.GetType(), propertyName);
                if (prop != null)
                    prop.SetValue(source, value);
            }
        }

        private static PropertyInfo SafeGetPropertyInfoRecursively(Type sourceType, string propertyName)
        {
            if (sourceType != null)
            {
                var typeInfo = sourceType.GetTypeInfo();
                var prop = typeInfo.GetDeclaredProperty(propertyName);
                if (prop != null)
                    return prop;

                return SafeGetPropertyInfoRecursively(typeInfo.BaseType, propertyName);
            }

            return null;
        }
    }
}
