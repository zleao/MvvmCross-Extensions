using System;
using System.Text.RegularExpressions;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Determines whether the specified source string is null or empty.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   <c>true</c> if the specified source string is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// Formats the specified source template.
        /// </summary>
        /// <param name="sourceTemplate">The source template.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static string FormatTemplate(this string sourceTemplate, params object[] args)
        {
            if (string.IsNullOrEmpty(sourceTemplate))
                return sourceTemplate ?? string.Empty;

            return string.Format(sourceTemplate, args);
        }

        /// <summary>
        /// Determines if source contains the specified value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if source contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string source, char value)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return source.Contains(value.ToString());
        }

        /// <summary>
        /// Parses the source string value to enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="sourceEnumValue">The source enum value.</param>
        /// <param name="ignoreCase">if set to <c>true</c> ignore case.</param>
        /// <returns></returns>
        public static TEnum Parse2Enum<TEnum>(this string sourceEnumValue, bool ignoreCase = false)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), sourceEnumValue, ignoreCase);
        }

        /// <summary>
        /// Parses the source string value to decimal.
        /// </summary>
        /// <param name="sourceValue">The source value.</param>
        /// <param name="valueIfNull">The value if null.</param>
        /// <param name="throwException">if set to <c>true</c> throw exception.</param>
        /// <returns></returns>
        public static decimal Parse2Decimal(this string sourceValue, decimal valueIfNull = 0M, bool throwException = false)
        {
            decimal d = valueIfNull;

            if (!sourceValue.IsNullOrEmpty())
            {
                try
                {
                    var trimmedSource = sourceValue.Replace(',', '.').Trim();
                    d = Convert.ToDecimal(trimmedSource, new System.Globalization.CultureInfo("en-US"));
                }
                catch (Exception)
                {
                    if (throwException)
                        throw;
                }
            }

            return d;
        }

        /// <summary>
        /// Parses the source string value to boolean.
        /// </summary>
        /// <param name="sourceValue">The source value.</param>
        /// <param name="defaultValue">The default value. Aplicable to null or empty string</param>
        /// <returns></returns>
        public static bool Parse2Boolean(this string sourceValue, bool defaultValue = false)
        {
            if (sourceValue.IsNullOrEmpty())
                return defaultValue;

            #region Test for true/false string

            if (sourceValue.Equals(bool.FalseString))
                return false;

            if (sourceValue.Equals(bool.TrueString))
                return true;

            #endregion

            #region Test for integer values

            int intSourceValue;
            if (int.TryParse(sourceValue, out intSourceValue))
            {
                return intSourceValue > 0;
            }

            #endregion

            try
            {
                return Convert.ToBoolean(sourceValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Parses the source string value to int32.
        /// </summary>
        /// <param name="sourceValue">The source value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int Parse2Int(this string sourceValue, int defaultValue = 0)
        {
            if (sourceValue.IsNullOrEmpty())
                return defaultValue;

            try
            {
                return Convert.ToInt32(sourceValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Truncates the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string Truncate(this string text, int length)
        {
            return text.Length > length ? text.ToString().Substring(0, length - 1) : text;
        }

        /// <summary>
        /// Determines whether the specified value is json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is json; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJson(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.Trim();

            return value.StartsWith("{") && value.EndsWith("}")
                || value.StartsWith("[") && value.EndsWith("]");
        }

        /// <summary>
        /// Gets the value if null.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string GetValueIfNull(this string text)
        {
            if (text.IsNullOrEmpty())
                return "";

            return text;
        }

        private static Regex _hexRegex = new Regex(@"^[0-9a-f]+$");
        /// <summary>
        /// Determines whether the specified value is hexadecimal.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        ///   <c>true</c> if the specified text is hex; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsHex(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return _hexRegex.IsMatch(value.Trim().ToLower());
        }

        /// <summary>
        /// Gets the binary from hex.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetBinaryFromHex(this string value)
        {
            if (!value.IsHex())
                return "";

            return Convert.ToString(Convert.ToInt32(value, 16), 2);
        }

        /// <summary>
        /// Trims outer spaces from the specified text.
        /// If text is null, return string.Emtpy
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string TrimOrEmpty(this string text)
        {
            if (text.IsNullOrEmpty())
                return string.Empty;

            return text.Trim();
        }

        /// <summary>
        /// Adds a line break to the text, if not empty.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string AddCrIfNotEmpty(this string text)
        {
            return text.IsNullOrEmpty() ? "" : text + "\n";
        }

        /// <summary>
        /// Removes white spaces from the text.
        /// If null or empty, returns String.Empty.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Trim(this string text)
        {
            if (text.IsNullOrEmpty())
                return string.Empty;
            else
                return text.Trim();
        }
    }
}
