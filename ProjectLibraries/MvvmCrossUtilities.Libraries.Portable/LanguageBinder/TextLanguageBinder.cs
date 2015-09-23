using Cirrious.MvvmCross.Localization;

namespace MvvmCrossUtilities.Libraries.Portable.LanguageBinder
{
    /// <summary>
    /// TextLanguageBinder
    /// </summary>
    public class TextLanguageBinder : MvxLanguageBinder, ITextLanguageBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextLanguageBinder"/> class.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="typeName">Name of the type.</param>
        public TextLanguageBinder(string namespaceName, string typeName) 
            : base(namespaceName, typeName)
        {

        }
    }
}
