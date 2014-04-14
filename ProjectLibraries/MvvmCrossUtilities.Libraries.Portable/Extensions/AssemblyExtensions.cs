using System.Reflection;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    public static class AssemblyExtensions
    {
        public static AssemblyName GetName(this Assembly source)
        {
            if (source == null)
                return null;

            return new AssemblyName(source.FullName);
        }
    }
}
