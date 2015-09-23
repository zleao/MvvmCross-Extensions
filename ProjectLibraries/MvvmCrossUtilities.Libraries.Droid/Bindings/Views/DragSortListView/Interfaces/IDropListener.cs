namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Interfaces
{
    /// <summary>
    /// Your implementation of this has to reorder your ListAdapter! 
    /// Make sure to call
    /// {@link BaseAdapter#notifyDataSetChanged()} or something like it
    /// in your implementation.
    /// </summary>
    public interface IDropListener
    {
        void Drop(int from, int to);
    }
}