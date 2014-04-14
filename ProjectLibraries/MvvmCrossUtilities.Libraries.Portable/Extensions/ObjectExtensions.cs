namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static object GetPropertyValue(this object source, string propertyName)
        {
            if (source == null)
                return null;

            return source.GetType().GetPropertyValue(propertyName);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public static void SetPropertyValue(this object source, string propertyName, object value)
        {
            if (source == null)
                return;

            source.GetType().SetPropertyValue(propertyName, value);
        }
    }
}
