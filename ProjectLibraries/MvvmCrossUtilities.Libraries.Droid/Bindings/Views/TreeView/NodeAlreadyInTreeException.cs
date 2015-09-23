using Java.Lang;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView
{
    //see: https://bugzilla.xamarin.com/show_bug.cgi?format=multiple&id=22959
    //public class NodeAlreadyInTreeException : RuntimeException
    public class NodeAlreadyInTreeException : Throwable
    {
        //private static readonly long serialVersionUID = 1L;

        public NodeAlreadyInTreeException(string id, string oldNode)
            : base("The node has already been added to the tree: " + id + ". Old node is:" + oldNode)
        {
        }
    }
}
