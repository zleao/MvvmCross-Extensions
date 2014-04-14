using Java.Lang;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView
{
    public class NodeAlreadyInTreeException : RuntimeException
    {
        private static readonly long serialVersionUID = 1L;

        public NodeAlreadyInTreeException(string id, string oldNode)
            : base("The node has already been added to the tree: " + id + ". Old node is:" + oldNode)
        {
        }
    }
}
