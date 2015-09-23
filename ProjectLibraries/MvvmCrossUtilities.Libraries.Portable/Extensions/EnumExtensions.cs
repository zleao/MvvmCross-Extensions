﻿using MvvmCrossUtilities.Libraries.Portable.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    /// <summary>
    /// Extensions for System.Enum type
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the int value of the enum.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public static int GetIntValue(this System.Enum e)
        {
            return Convert.ToInt32(e);
        }

        /// <summary>
        /// Determines whether the enum type has the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [has] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether value is of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Adds the specified value to the enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

        /// <summary>
        /// Removes the specified value to from the enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

        /// <summary>
        /// Gets a dictionary with the enumerations options and the code of the StringValueAttribute associated with each option.
        /// If the option doesn't have a attribute associated, it returns the name of the option
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Dictionary<string, Enum> GetAttributeCodeKeyDictionary(this System.Enum type)
        {
            Type enumType = type.GetType();
            Type underlyingType = Enum.GetUnderlyingType(enumType);

            Dictionary<string, Enum> result = new Dictionary<string, Enum>();

            var enumerationFields = enumType.GetTypeInfo().DeclaredFields.Where(f => f.IsStatic && f.IsPublic);

            foreach (FieldInfo fieldInfo in enumerationFields)
            {
                StringValueAttribute[] attrs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attrs.Length > 0)
                    result.Add(attrs[0].Code, (Enum)Enum.Parse(enumType, fieldInfo.Name, false));
                else
                    result.Add(fieldInfo.Name, (Enum)Enum.Parse(enumType, fieldInfo.Name, false));
            }

            return result;
        }

        /// <summary>
        /// Gets a dictionary with the enumerations options and the value of the StringValueAttribute associated with each option.
        /// If the option doesn't have a attribute associated, it returns the name of the option
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Dictionary<string, Enum> GetAttributeValueKeyDictionary(this System.Enum type)
        {
            Type enumType = type.GetType();
            Type underlyingType = Enum.GetUnderlyingType(enumType);

            Dictionary<string, Enum> result = new Dictionary<string, Enum>();

            var enumerationFields = enumType.GetTypeInfo().DeclaredFields.Where(f => f.IsStatic && f.IsPublic);

            foreach (FieldInfo fieldInfo in enumerationFields)
            {
                StringValueAttribute[] attrs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attrs.Length > 0)
                    result.Add(attrs[0].Value, (Enum)Enum.Parse(enumType, fieldInfo.Name, false));
                else
                    result.Add(fieldInfo.Name, (Enum)Enum.Parse(enumType, fieldInfo.Name, false));
            }

            return result;
        }

        /// <summary>
        /// Gets the enum values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns></returns>
        public static IList<T> GetEnumValues<T>(this Type enumType)
        {
            Type underlyingType = Enum.GetUnderlyingType(enumType);

            List<T> result = new List<T>();

            var enumerationFields = enumType.GetTypeInfo().DeclaredFields.Where(f => f.IsStatic && f.IsPublic);

            foreach (FieldInfo fieldInfo in enumerationFields)
            {
                StringValueAttribute[] attrs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attrs.Length > 0)
                    result.Add((T)Enum.Parse(enumType, fieldInfo.Name, false));
                else
                    result.Add((T)Enum.Parse(enumType, fieldInfo.Name, false));
            }

            return result;
        }
    }
}
