using System;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Formats the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string Format(this DateTime? source, string format)
        {
            if (source == null || !source.HasValue || string.IsNullOrEmpty(format))
                return string.Empty;

            return source.Value.ToString(format);
        }

        /// <summary>
        /// Retunrs the date part of the DateTime structure
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static DateTime ToDate(this DateTime t)
        {
            return new DateTime(t.Year, t.Month, t.Day);
        }

        /// <summary>
        /// Gets the UTC adjusted time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static DateTime GetUtcAdjustedTime(this DateTime date)
        {
            return new DateTime(date.Ticks, DateTimeKind.Utc);
        }


        /// <summary>
        /// Compares the value of this instance to a specified System.DateTime value
        /// and returns an integer that indicates whether this instance is earlier than,
        /// the same as, or later than the specified System.DateTime value.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static int CompareTo(this DateTime date, DateTime? dateToCompare)
        {
            DateTime safeToCompare = DateTime.MinValue;
            if (dateToCompare != null || dateToCompare.HasValue)
                safeToCompare = dateToCompare.Value;

            return date.CompareTo(safeToCompare);
        }

        /// <summary>
        /// Converts the specified string representation of a date and time to its System.DateTime
        /// equivalent using the specified format and culture-specific format information.
        /// The format of the string representation must match the specified format exactly.
        /// </summary>
        /// <param name="s">The string to convert</param>
        /// <param name="format">The format to apply</param>
        /// <param name="provider">The culture-specific format</param>
        /// <returns></returns>
        public static DateTime TryParseExact(string s, string format, IFormatProvider provider)
        {
            try
            {
                return DateTime.ParseExact(s, format, provider);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }
}
