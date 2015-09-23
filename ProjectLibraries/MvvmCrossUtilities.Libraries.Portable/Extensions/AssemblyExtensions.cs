using System.Reflection;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    /// <summary>
    /// Extensions for Assembly type
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the name of the assembly.
        /// Deals with null source
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static AssemblyName SafeGetName(this Assembly source)
        {
            if (source == null)
                return null;

            return new AssemblyName(source.FullName);
        }
    }
}
