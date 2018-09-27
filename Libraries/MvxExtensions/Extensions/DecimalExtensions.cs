using System;

namespace MvxExtensions.Extensions
{
    /// <summary>
    /// Extensions ofr decimal type
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// Rounds a decimal value with a precision of 2 decimal places.
        /// </summary>
        /// <param name="value">The decimal value to round</param>
        /// <returns></returns>
        public static decimal RoundsCurrency(this decimal value)
        {
            return RoundsCurrency(value, 2);
        }

        /// <summary>
        /// Rounds a decimal value with a precision defined by argument.
        /// </summary>
        /// <param name="value">The decimal value to round</param>
        /// <param name="precision">Decimal places precision.</param>
        /// <returns></returns>
        public static decimal RoundsCurrency(this decimal value, int precision)
        {
            value = value * (decimal)(Math.Pow(10, precision));

            if (value - (long)value >= 0.5m)
            {
                value = (long)value + 1;
            }
            else
            {
                value = (long)value;
            }
            return value / (decimal)(Math.Pow(10, precision));
        }
    }
}
