using Java.Lang;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView
{
    /**
     * Exception thrown when there is a problem with configuring tree.
     * 
     */
    public class TreeConfigurationException : RuntimeException
    {

        private static long serialVersionUID = 1L;

        public TreeConfigurationException(string detailMessage)
            : base(detailMessage)
        {
        }
    }
}
