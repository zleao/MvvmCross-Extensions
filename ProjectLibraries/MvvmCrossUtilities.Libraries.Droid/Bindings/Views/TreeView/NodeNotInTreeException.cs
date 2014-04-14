using Java.Lang;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView
{
    public class NodeNotInTreeException : RuntimeException
    {
        private static readonly long serialVersionUID = 1L;

        public NodeNotInTreeException(string id)
            : base("The tree does not contain the node specified: " + id)
        {
        }

    }

}
