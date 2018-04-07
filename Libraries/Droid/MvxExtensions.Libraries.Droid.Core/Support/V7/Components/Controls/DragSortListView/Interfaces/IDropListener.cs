namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView.Interfaces
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