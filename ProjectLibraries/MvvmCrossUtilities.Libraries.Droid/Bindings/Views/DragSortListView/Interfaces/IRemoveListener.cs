namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Interfaces
{
    /// <summary>
    /// Make sure to call
    /// {@link BaseAdapter#notifyDataSetChanged()} or something like it
    /// in your implementation.
    /// </summary>
    public interface IRemoveListener
    {
        void Remove(int which);
    }
}