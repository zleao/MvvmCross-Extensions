using Android.Content;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Java.Lang;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView;
using MvvmCrossUtilities.Libraries.Portable.Models;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters.TreeView
{
    [Deprecated]
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

        #region Original Java code

        //private IList<long> selected;

        //private final OnCheckedChangeListener onCheckedChange = new OnCheckedChangeListener() {
        //    @Override
        //    public void onCheckedChanged(final CompoundButton buttonView,
        //            final boolean isChecked) {
        //        final Long id = (Long) buttonView.getTag();
        //        changeSelected(isChecked, id);
        //    }

        //};

        //private void ChangeSelected(bool isChecked, long id)
        //{
        //    if (isChecked)
        //    {
        //        selected.Add(id);
        //    }
        //    else
        //    {
        //        selected.Remove(id);
        //    }
        //}

        //private String getDescription(final long id) {
        //    final Integer[] hierarchy = getManager().getHierarchyDescription(id);
        //    return "Node " + id + Arrays.asList(hierarchy);
        //}

        //@Override
        //public View getNewChildView(final TreeNodeInfo<Long> treeNodeInfo) {
        //    final LinearLayout viewLayout = (LinearLayout) getActivity()
        //            .getLayoutInflater().inflate(R.layout.demo_list_item, null);
        //    return updateView(viewLayout, treeNodeInfo);
        //}

        //@Override
        //public LinearLayout updateView(final View view,
        //        final TreeNodeInfo<Long> treeNodeInfo) {
        //    final LinearLayout viewLayout = (LinearLayout) view;
        //    final TextView descriptionView = (TextView) viewLayout
        //            .findViewById(R.id.demo_list_item_description);
        //    final TextView levelView = (TextView) viewLayout
        //            .findViewById(R.id.demo_list_item_level);
        //    descriptionView.setText(getDescription(treeNodeInfo.getId()));
        //    levelView.setText(Integer.toString(treeNodeInfo.getLevel()));
        //    final CheckBox box = (CheckBox) viewLayout
        //            .findViewById(R.id.demo_list_checkbox);
        //    box.setTag(treeNodeInfo.getId());
        //    if (treeNodeInfo.isWithChildren()) {
        //        box.setVisibility(View.GONE);
        //    } else {
        //        box.setVisibility(View.VISIBLE);
        //        box.setChecked(selected.contains(treeNodeInfo.getId()));
        //    }
        //    box.setOnCheckedChangeListener(onCheckedChange);
        //    return viewLayout;
        //}

        //@Override
        //public void handleItemClick(final View view, final Object id) {
        //    final Long longId = (Long) id;
        //    final TreeNodeInfo<Long> info = getManager().getNodeInfo(longId);
        //    if (info.isWithChildren()) {
        //        super.handleItemClick(view, id);
        //    } else {
        //        final ViewGroup vg = (ViewGroup) view;
        //        final CheckBox cb = (CheckBox) vg
        //                .findViewById(R.id.demo_list_checkbox);
        //        cb.performClick();
        //    }
        //}

        //#region Fields

        //private IDisposable _subscription;

        //#endregion

        #endregion
    }
}