using System;
using System.Text;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFullDescription(this Exception ex)
        {
            StringBuilder desc = new StringBuilder(ex.Message);

            if (ex.InnerException != null)
            {
                desc.Append(" + ");
                desc.Append(GetFullDescription(ex.InnerException));
            }

            return desc.ToString();
        }
    }
}
