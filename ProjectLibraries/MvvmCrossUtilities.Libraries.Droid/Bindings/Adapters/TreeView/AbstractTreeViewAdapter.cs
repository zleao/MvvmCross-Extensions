using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using Android.Content;
using Android.Database;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore.Exceptions;
using Cirrious.CrossCore.Platform;
using Cirrious.CrossCore.WeakSubscription;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView;
using MvvmCrossUtilities.Libraries.Portable.Extensions;
using MvvmCrossUtilities.Libraries.Portable.Models;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters.TreeView
{
    public abstract class AbstractTreeViewAdapter<T> : BaseAdapter, IListAdapter, Android.Views.View.IOnClickListener
        where T : class, IExpandable
    {
        #region Constants

        public const string IMAGE_MULTI_SELECTION_TAG = "MultiSelectionImage";
        public const string IMAGE_SINGLE_SELECTION_TAG = "SingleSelectionImage";

        #endregion

        #region Fields

        private readonly string TAG = "AbstractTreeViewAdapter";
        private readonly int _numberOfLevels;

        private int _indentWidth;
        private GravityFlags _indicatorGravity = GravityFlags.NoGravity;
        private bool _collapsible;
        private bool _selectionEnabled;
        
        private Drawable _collapsedDrawable;
        private Drawable _expandedDrawable;
        private Drawable _indicatorBackgroundDrawable;
        private Drawable _rowBackgroundDrawable;

        private MvxNotifyCollectionChangedEventSubscription _collectionChangedToken = null;

        #endregion

        #region Properties

        protected Context Context
        {
            get { return _context; }
        }
        private readonly Context _context;

        protected IMvxAndroidBindingContext BindingContext
        {
            get { return _bindingContext; }
        }
        private readonly IMvxAndroidBindingContext _bindingContext;

        public ITreeStateManager<T> TreeStateManager
        {
            get { return _treeStateManager; }
        }
        private ITreeStateManager<T> _treeStateManager;

        public override int Count
        {
            get { return _treeStateManager.GetVisibleCount(); }
        }

        public override bool HasStableIds
        {
            get { return true; }
        }

        public override int ViewTypeCount
        {
            get { return _numberOfLevels; }
        }

        public override bool IsEmpty
        {
            get { return Count == 0; }
        }

        public virtual int ItemTemplateId
        {
            get { return _itemTemplateId; }
            set
            {
                if (_itemTemplateId == value)
                    return;
                _itemTemplateId = value;

                // since the template has changed then let's force the list to redisplay by firing NotifyDataSetChanged()
                NotifyDataSetChanged();
            }
        }
        private int _itemTemplateId;

        public bool PropagateSelection
        {
            get { return _propagateSelection; }
            set { _propagateSelection = value; }
        }
        private bool _propagateSelection = true;

        public bool ExpandChildWhenSelected
        {
            get { return _expandChildWhenSelected; }
            set { _expandChildWhenSelected = value; }
        }
        private bool _expandChildWhenSelected = true;

        public bool AllowParentSelection
        {
            get { return _allowParentSelection; }
            set { SetAllowParentSelection(value); }
        }
        private bool _allowParentSelection = true;

        public bool SingleSelection
        {
            get { return _singleSelection; }
            set { SetSingleSelection(value); }
        }
        private bool _singleSelection = false;

        public IList SelectedItems
        {
            get { return _selectedItems; }
            set { SetSelectedItems(value); }
        }
        private IList _selectedItems;

        private bool _ignoreSelectedItemsChanged = false;

        #endregion

        #region Constructor

        public AbstractTreeViewAdapter(Context context, ITreeStateManager<T> treeStateManager, int numberOfLevels, int itemTemplateId)
            : this(context, MvxAndroidBindingContextHelpers.Current(), treeStateManager, numberOfLevels, itemTemplateId)
        { 
        }

        public AbstractTreeViewAdapter(Context context, IMvxAndroidBindingContext bindingContext, ITreeStateManager<T> treeStateManager, int numberOfLevels, int itemTemplateId) 
        {
            _itemTemplateId = itemTemplateId;
            _context = context;
            _bindingContext = bindingContext;
            if (_bindingContext == null)
                throw new MvxException("TreeViewList can only be used within a Context which supports IMvxBindingActivity");

            _treeStateManager = treeStateManager;
            _numberOfLevels = numberOfLevels;

            this._collapsedDrawable = null;
            this._expandedDrawable = null;
            this._rowBackgroundDrawable = null;
            this._indicatorBackgroundDrawable = null;
        }

        #endregion

        #region Overriden Methods

        public override void RegisterDataSetObserver(DataSetObserver observer)
        {
            _treeStateManager.RegisterDataSetObserver(observer);
        }

        public override void UnregisterDataSetObserver(DataSetObserver observer)
        {
            _treeStateManager.UnregisterDataSetObserver(observer);
        }

        public override bool AreAllItemsEnabled()
        {
            // NOPMD
            return true;
        }

        public override bool IsEnabled(int position)
        {
            return true;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Log.Debug(TAG, "Creating a view based on " + convertView + " with position " + position);
            
            TreeNodeInfo<T> nodeInfo = GetTreeNodeInfo(position);

            var bindableViewToUse = convertView as ITreeViewListItem;
            if (bindableViewToUse != null)
            {
                if (bindableViewToUse.TemplateId != GetTreeListItemWrapperId())
                {
                    bindableViewToUse = null;
                }
            }

            var newChild = false;
            var dataContext = nodeInfo.GetId();

            if (bindableViewToUse == null)
            {
                newChild = true;
                bindableViewToUse = new BindableTreeViewListItem(Context, BindingContext.LayoutInflater, dataContext, GetTreeListItemWrapperId());
            }
            else
            {
                bindableViewToUse.DataContext = dataContext;
            }

            if (SelectedItems == null || !SelectedItems.Contains(dataContext))
            {
                bindableViewToUse.Checked = false;
            }
            else
            {
                bindableViewToUse.Checked = true;
            }

            return PopulateTreeItem(bindableViewToUse as BindableTreeViewListItem, nodeInfo, newChild);
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int GetItemViewType(int position)
        {
            return GetTreeNodeInfo(position).GetLevel();
        }

        #endregion

        #region Methods

        public void UpdateTree()
        {
            NotifyDataSetChanged();
        }

        protected void ExpandCollapse(T id)
        {
            TreeNodeInfo<T> nodeInfo = _treeStateManager.GetNodeInfo(id);
            if (!nodeInfo.IsWithChildren())
            {
                // ignore - no default action
                return;
            }
            if (nodeInfo.IsExpanded())
            {
                _treeStateManager.CollapseChildren(id);
            }
            else
            {
                HandleChildrenLazyLoadIfNeeded(id);
                _treeStateManager.ExpandDirectChildren(id);
            }
        }
       
        private void HandleChildrenLazyLoadIfNeeded(T id)
        {
            if (id.HasChildren && id.Children.IsNullOrEmpty())
            {
                id.GetChildren();
                var addChildrenToSelected = (!SingleSelection && SelectedItems.Contains(id));
                var treeBuilder = new TreeBuilder<T>(TreeStateManager);

                _ignoreSelectedItemsChanged = true;

                foreach (var item in id.Children)
                {
                    treeBuilder.AddRelation(id, item as T);
                    if (addChildrenToSelected)
                        OnSelectItem(item as T, addChildrenToSelected);
                }

                _ignoreSelectedItemsChanged = false;
            }
        }

        private void CalculateIndentWidth()
        {
            if (_collapsedDrawable != null)
            {
                _indentWidth = Math.Max(GetIndentWidth(), _collapsedDrawable.IntrinsicWidth);
            }
            else if (_expandedDrawable != null)
            {
                _indentWidth = Math.Max(GetIndentWidth(), _expandedDrawable.IntrinsicWidth);
            }
        }

        public T GetTreeId(int position)
        {
            return _treeStateManager.GetVisibleList().ElementAt(position);
        }

        public virtual TreeNodeInfo<T> GetTreeNodeInfo(int position)
        {
            return _treeStateManager.GetNodeInfo(GetTreeId(position));
        }

        protected int GetTreeListItemWrapperId()
        {
            return Resource.Layout.tree_list_item_wrapper;
        }

        public View PopulateTreeItem(BindableTreeViewListItem bindableView, TreeNodeInfo<T> nodeInfo, bool newChildView)
        {
            Drawable individualRowDrawable = GetBackgroundDrawable(nodeInfo);

            bindableView.Background = (individualRowDrawable == null ? GetDrawableOrDefaultBackground(_rowBackgroundDrawable) : individualRowDrawable);

            LinearLayout.LayoutParams indicatorLayoutParams = new LinearLayout.LayoutParams(CalculateIndentation(nodeInfo), ViewGroup.LayoutParams.MatchParent);

            LinearLayout indicatorLayout = bindableView.FindViewById<LinearLayout>(Resource.Id.treeview_list_item_image_layout);
            indicatorLayout.SetGravity(_indicatorGravity);
            indicatorLayout.LayoutParameters = indicatorLayoutParams;

            ImageView image = bindableView.FindViewById<ImageView>(Resource.Id.treeview_list_item_image);
            image.SetImageDrawable(GetDrawable(nodeInfo));
            image.Background = GetDrawableOrDefaultBackground(_indicatorBackgroundDrawable);
            image.SetScaleType(Android.Widget.ImageView.ScaleType.Center);

            bindableView.SetOnClickListener(this);

            if (newChildView)
            {
                FrameLayout frameLayout = bindableView.FindViewById<FrameLayout>(Resource.Id.treeview_list_item_frame);
                var childView = bindableView.BindingInflate(ItemTemplateId, frameLayout);
            }


            var selectorVisible = _selectionEnabled && !(nodeInfo.IsWithChildren() && !AllowParentSelection);

            var singleImage = bindableView.FindViewWithTag(IMAGE_SINGLE_SELECTION_TAG);
            if (singleImage != null)
            {
                singleImage.Visibility = (selectorVisible && SingleSelection ? ViewStates.Visible : ViewStates.Gone);
                singleImage.SetOnClickListener(singleImage.Visibility == ViewStates.Visible ? this : null);
            }

            var multiImage = bindableView.FindViewWithTag(IMAGE_MULTI_SELECTION_TAG);
            if (multiImage != null)
            {
                multiImage.Visibility = (selectorVisible && !SingleSelection ? ViewStates.Visible : ViewStates.Gone);
                multiImage.SetOnClickListener(multiImage.Visibility == ViewStates.Visible ? this : null);
            }

            return bindableView;
        }

        public Drawable GetBackgroundDrawable(TreeNodeInfo<T> treeNodeInfo)
        {
            // NOPMD
            return null;
        }

        private Drawable GetDrawableOrDefaultBackground(Drawable r)
        {
            if (r == null)
            {
                return Context.Resources.GetDrawable(Resource.Drawable.list_selector_background).Mutate();
            }
            else
            {
                return r;
            }
        }

        protected int CalculateIndentation(TreeNodeInfo<T> nodeInfo)
        {
            return GetIndentWidth() * (nodeInfo.GetLevel() + (_collapsible ? 1 : 0));
        }

        protected Drawable GetDrawable(TreeNodeInfo<T> nodeInfo)
        {
            if (!nodeInfo.IsWithChildren() || !_collapsible)
            {
                return GetDrawableOrDefaultBackground(_indicatorBackgroundDrawable);
            }

            if (nodeInfo.IsExpanded())
            {
                return _expandedDrawable;
            }
            else
            {
                return _collapsedDrawable;
            }
        }

        public void SetIndicatorGravity(GravityFlags indicatorGravity)
        {
            this._indicatorGravity = indicatorGravity;
        }

        public void SetCollapsedDrawable(Drawable collapsedDrawable)
        {
            this._collapsedDrawable = collapsedDrawable;
            CalculateIndentWidth();
        }

        public void SetExpandedDrawable(Drawable expandedDrawable)
        {
            this._expandedDrawable = expandedDrawable;
            CalculateIndentWidth();
        }

        public void SetIndentWidth(int indentWidth)
        {
            this._indentWidth = indentWidth;
            CalculateIndentWidth();
        }

        public void SetRowBackgroundDrawable(Drawable rowBackgroundDrawable)
        {
            this._rowBackgroundDrawable = rowBackgroundDrawable;
        }

        public void SetIndicatorBackgroundDrawable(Drawable indicatorBackgroundDrawable)
        {
            this._indicatorBackgroundDrawable = indicatorBackgroundDrawable;
        }

        public void SetCollapsible(bool collapsible)
        {
            this._collapsible = collapsible;
        }

        public void SetSelectionEnabled(bool selectionEnabled)
        {
            this._selectionEnabled = selectionEnabled;
        }

        public void Refresh()
        {
            _treeStateManager.Refresh();
        }

        private int GetIndentWidth()
        {
            return _indentWidth;
        }


        protected virtual void SetAllowParentSelection(bool value)
        {
            if (_allowParentSelection != value)
            {
                _allowParentSelection = value;

                if (!_allowParentSelection && SelectedItems != null)
                {
                    _ignoreSelectedItemsChanged = true;
                    SelectedItems.Clear();
                    _ignoreSelectedItemsChanged = false;
                }

                _treeStateManager.Refresh();
            }
        }

        protected virtual void SetSingleSelection(bool value)
        {
            if (_singleSelection != value)
            {
                _singleSelection = value;

                if (_singleSelection && SelectedItems != null && SelectedItems.Count > 1)
                {
                    _ignoreSelectedItemsChanged = true;
                    SelectedItems.Clear();
                    _ignoreSelectedItemsChanged = false;
                }

                _treeStateManager.Refresh();
            }
        }

        public void SetSelectedItems(IList items)
        {
            if (_collectionChangedToken != null)
            {
                _collectionChangedToken.Dispose();
                _collectionChangedToken = null;
            }

            if (items == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to SelectableListView.SelectedItems binding");
                return;
            }

            if (typeof(INotifyCollectionChanged).IsAssignableFrom(items.GetType()))
            {
                _collectionChangedToken = (items as INotifyCollectionChanged).WeakSubscribe(OnSelectedItemsCollectionChanged);
            }

            _selectedItems = items;
        }

        private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (!_ignoreSelectedItemsChanged)
                {
                    //switch (e.Action)
                    //{
                    //    case NotifyCollectionChangedAction.Add:
                    //        foreach (var item in e.NewItems)
                    //        {
                    //            var itemPosition = Adapter.GetPosition(item);
                    //            if (itemPosition >= 0)
                    //                this.SetItemChecked(itemPosition, true);
                    //        }
                    //        break;

                    //    case NotifyCollectionChangedAction.Move:
                    //        Android.Util.Log.Error("SelectableListView.OnSelectedItemsCollectionChanged", "NotifyCollectionChangedAction.Move not supported");
                    //        break;

                    //    case NotifyCollectionChangedAction.Remove:
                    //        foreach (var item in e.OldItems)
                    //        {
                    //            var itemPosition = Adapter.GetPosition(item);
                    //            if (itemPosition >= 0)
                    //                this.SetItemChecked(itemPosition, false);
                    //        }
                    //        break;

                    //    case NotifyCollectionChangedAction.Replace:
                    //        Android.Util.Log.Error("SelectableListView.OnSelectedItemsCollectionChanged", "NotifyCollectionChangedAction.Replace not supported");
                    //        break;

                    //    case NotifyCollectionChangedAction.Reset:
                    //        for (int i = 0; i < Adapter.Count; i++)
                    //        {
                    //            this.SetItemChecked(i, false);
                    //        }
                    //        break;

                    //    default:
                    //        break;
                    //}

                    _treeStateManager.Refresh();
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Wtf("SelectableListView.OnSelectedItemsCollectionChanged", ex.Message);
            }
        }

        private void OnSelectItem(T selectedItem, bool currentSelectionState)
        {
            if (selectedItem == null)
            {
                Log.Debug(TAG, "OnSelectedItem.SelectedItem is null.");
                return;
            }

            if (SelectedItems == null)
            {
                Log.Debug(TAG, "OnSelectedItem.SelectedItems is null.");
                return;
            }

            var expandEverythingBelow = false;
            _ignoreSelectedItemsChanged = true;

            if (SingleSelection)
            {
                SelectedItems.Clear();

                if (currentSelectionState)
                    SelectedItems.Add(selectedItem);
            }
            else
            {
                if (currentSelectionState)
                {
                    if (!SelectedItems.Contains(selectedItem))
                        SelectedItems.Add(selectedItem);

                    if (PropagateSelection)
                        expandEverythingBelow = true; //To force the expand of child items
                }
                else if (SelectedItems.Contains(selectedItem))
                {
                    SelectedItems.Remove(selectedItem);
                }

                if (PropagateSelection)
                    AlignSelection(selectedItem, currentSelectionState);
            }

            _ignoreSelectedItemsChanged = false;

            if (expandEverythingBelow && ExpandChildWhenSelected)
            {
                HandleChildrenLazyLoadIfNeeded(selectedItem);
                _treeStateManager.ExpandEverythingBelow(selectedItem);
            }
            else
            {
                _treeStateManager.Refresh();
            }
        }

        private void AlignSelection(T selectedItem, bool currentSelectionState)
        {
            //We should only propagate deselection to the parent.
            //The parent does not need to know when a child is selected
            if (currentSelectionState == false)
                PropagateDeselectionToParent(selectedItem);

            PropagateSelectionStateToChild(selectedItem, currentSelectionState);
        }

        private void PropagateDeselectionToParent(T item)
        {
            var parent = _treeStateManager.GetParent(item);
            if (parent != null && SelectedItems.Contains(parent))
            {
                SelectedItems.Remove(parent);
                PropagateDeselectionToParent(parent);
            }
        }

        private void PropagateSelectionStateToChild(T selectedItem, bool currentSelectionState)
        {
            if (selectedItem == null)
            {
                Log.Debug(TAG, "PropagateSelectionStateToChild.SelectedItem is null.");
                return;
            }

            if (SelectedItems == null)
            {
                Log.Debug(TAG, "PropagateSelectionStateToChild.SelectedItems is null.");
                return;
            }

            var children = _treeStateManager.GetChildren(selectedItem);
            if (children != null)
            {
                foreach (var item in children)
                {
                    if (currentSelectionState)
                    {
                        if (!SelectedItems.Contains(item))
                            SelectedItems.Add(item);
                    }
                    else 
                    {
                        if (SelectedItems.Contains(item))
                            SelectedItems.Remove(item);
                    }

                    PropagateSelectionStateToChild(item, currentSelectionState);
                }
            }
        }

        #endregion

        #region IOnClickListener Members

        public void OnClick(View v)
        {
            var bindableView = v as BindableTreeViewListItem;
            if (bindableView != null)
            {
                var nodeInfo = _treeStateManager.GetNodeInfo(bindableView.DataContext as T);
                if (nodeInfo != null && !nodeInfo.IsWithChildren())
                {
                    //Select or unselect leaf
                    OnSelectItem(bindableView.DataContext as T, !bindableView.Checked);
                }
                else if(_collapsible)
                {
                    //Expand or collapse the node
                    ExpandCollapse(bindableView.DataContext as T);
                }
            }
            else
            {
                var imageView = v as ImageView;
                if (imageView != null && imageView.Parent != null)
                {
                    //Select or unselect the item
                    var bindableListItem = imageView.Parent.Parent as BindableTreeViewListItem;
                    if (bindableListItem != null)
                    {
                        OnSelectItem(bindableListItem.DataContext as T, !bindableListItem.Checked);
                    }
                }
            }
        }

        #endregion
    }
}