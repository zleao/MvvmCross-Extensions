using System;
using System.Reflection;

namespace MvxExtensions.Extensions
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

        /// <summary>
        /// Checks if the IO component is null and throws <see cref="ArgumentNullException"/>
        /// </summary>
        /// <typeparam name="T">Type of the argument</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static T ThrowIfIoComponentIsNull<T>(this T argument, string propertyName)
        {
            if(argument == null)
                throw new ArgumentNullException($"{propertyName} is null - check if {typeof(T).Name} is registered in the IO container");

            return argument;
        }
    }
}
