using Android.Views;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.ExpandableRecyclerViewComponents
{
    public class ChildViewHolder : MvxRecyclerViewHolder
    {
        #region Constructor

        public ChildViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
        }

        #endregion
    }
}