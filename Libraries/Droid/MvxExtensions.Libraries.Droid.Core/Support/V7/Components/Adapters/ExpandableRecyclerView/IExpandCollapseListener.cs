namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Adapters.ExpandableRecyclerView
{
    /// <summary>
    /// Allows objects to register themselves as expand/collapse listeners to be notified of change events.
    /// Implement this in your <see cref="Android.App.Activity"/> or <see cref="Android.App.Fragment"/> to receive these callbacks.
    /// </summary>
    public interface IExpandCollapseListener
    {
        /// <summary>
        /// Called when a list item is expanded.
        /// </summary>
        /// <param name="position">The index of the item in the list being expanded</param>
        void OnListItemExpanded(int position);

        /// <summary>
        /// Called when a list item is collapsed.
        /// </summary>
        /// <param name="position">The index of the item in the list being collapsed</param>
        void OnListItemCollapsed(int position);
    }
}