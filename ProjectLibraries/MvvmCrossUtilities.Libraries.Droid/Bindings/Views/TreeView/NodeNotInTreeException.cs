using Java.Lang;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView
{
    //see: https://bugzilla.xamarin.com/show_bug.cgi?format=multiple&id=22959
    //public class NodeNotInTreeException : RuntimeException
    public class NodeNotInTreeException : Throwable
    {
        //private static readonly long serialVersionUID = 1L;

        public NodeNotInTreeException(string id)
            : base("The tree does not contain the node specified: " + id)
        {
        }

    }

}
