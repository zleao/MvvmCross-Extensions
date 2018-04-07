using Android.Views;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.ExpandableRecyclerViewComponents;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.SelectableRecyclerViewComponents;
using MvxExtensions.Libraries.Portable.Core.Models;
using MvvmCross.Binding.Droid.BindingContext;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Adapters.ExpandableRecyclerView
{
    public class ExpandableRecyclerViewAdapter : BaseExpandableRecyclerViewAdapter<ParentViewHolder, SelectableViewHolder>
    {
        #region Constructor

        public ExpandableRecyclerViewAdapter()
            : this(MvxAndroidBindingContextHelpers.Current())
        {
        }

        public ExpandableRecyclerViewAdapter(IMvxAndroidBindingContext bindingContext)
            : base(bindingContext)
        {
        }

        #endregion

        #region Override Methods

        public override ParentViewHolder OnCreateParentViewHolder(ViewGroup parentViewGroup)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parentViewGroup.Context, BindingContext.LayoutInflaterHolder);

            return new ParentViewHolder(InflateViewForHolder(parentViewGroup, TYPE_PARENT, itemBindingContext), itemBindingContext);
        }

        public override SelectableViewHolder OnCreateChildViewHolder(ViewGroup childViewGroup)
        {
            var itemBindingContext = new MvxAndroidBindingContext(childViewGroup.Context, BindingContext.LayoutInflaterHolder);

            return new SelectableViewHolder(InflateViewForHolder(childViewGroup, TYPE_CHILD, itemBindingContext), itemBindingContext);
        }

        protected override View InflateViewForHolder(ViewGroup parent, int viewType, IMvxAndroidBindingContext bindingContext)
        {
            if (viewType == TYPE_CHILD)
                return bindingContext.BindingInflate(ChildTemplateId, parent, false);

            return base.InflateViewForHolder(parent, viewType, bindingContext);
        }

        public override void OnBindParentViewHolder(ParentViewHolder parentViewHolder, int position, IGroupItem parentListItem)
        {
            parentViewHolder.DataContext = parentListItem;
        }

        public override void OnBindChildViewHolder(SelectableViewHolder childViewHolder, int position, object childListItem)
        {
            childViewHolder.DataContext = childListItem;
        }

        #endregion
    }
}