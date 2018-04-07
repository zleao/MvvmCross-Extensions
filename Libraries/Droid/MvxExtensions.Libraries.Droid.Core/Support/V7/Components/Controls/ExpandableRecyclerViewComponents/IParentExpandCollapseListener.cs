namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.ExpandableRecyclerViewComponents
{
    /// <summary>
    /// Empowers <see cref="Adapters.ExpandableRecyclerViewAdapter"/>
    /// </summary>
    public interface IParentExpandCollapseListener
    {
        void OnParentExpand(ParentViewHolder parentViewHolder);

        void OnParentCollapse(ParentViewHolder parentViewHolder);
    }
}