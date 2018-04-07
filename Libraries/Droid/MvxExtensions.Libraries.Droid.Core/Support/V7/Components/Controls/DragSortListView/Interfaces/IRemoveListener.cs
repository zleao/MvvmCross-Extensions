namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView.Interfaces
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