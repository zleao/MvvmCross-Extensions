using Android.Content;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView;
using MvvmCrossUtilities.Libraries.Portable.Models;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters.TreeView
{
    public class BindableTreeViewAdapter : AbstractTreeViewAdapter<IExpandable>
    {
        #region Constructor

        public BindableTreeViewAdapter(Context context, ITreeStateManager<IExpandable> treeStateManager, int numberOfLevels, int itemtemplateId)
            : base(context, treeStateManager, numberOfLevels, itemtemplateId)
        {
        }

        public BindableTreeViewAdapter(Context context, IMvxAndroidBindingContext bindingContext, ITreeStateManager<IExpandable> treeStateManager, int numberOfLevels, int itemtemplateId)
            : base(context, bindingContext, treeStateManager, numberOfLevels, itemtemplateId)
        {
        }

        #endregion
    }
}