using Android.Support.V7.Widget;
using Android.Views;
using Java.Lang;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.ExpandableRecyclerViewComponents;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.SelectableRecyclerViewComponents;
using MvxExtensions.Libraries.Portable.Core.Models;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Binding.ExtensionMethods;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform.WeakSubscription;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Adapters.ExpandableRecyclerView
{
    public abstract class BaseExpandableRecyclerViewAdapter<PVH, CVH> : MvxRecyclerAdapter, IParentExpandCollapseListener
         where PVH : ParentViewHolder
         where CVH : SelectableViewHolder
    {
        #region Constants

        private const string EXPANDED_STATE_MAP = "ExpandableRecyclerViewAdapter.ExpandedStateMap";
        protected const int TYPE_PARENT = 0;
        protected const int TYPE_CHILD = 1;

        #endregion

        #region Fields

        private bool _isExpanding;
        private bool _isCollapsing;

        private readonly List<MvxNotifyPropertyChangedEventSubscription> _propertyChangedSubscriptions = new List<MvxNotifyPropertyChangedEventSubscription>();

        private readonly List<MvxNotifyCollectionChangedEventSubscription> _itemsSourceChildCollectionChangedTokenList = new List<MvxNotifyCollectionChangedEventSubscription>();

        private MvxNotifyCollectionChangedEventSubscription _selectedItemscollectionChangedToken = null;

        protected readonly ExpandableSelectableViewHolderManager _selectableViewHolderManager = new ExpandableSelectableViewHolderManager();

        #endregion

        #region Properties

        public virtual int ChildTemplateId
        {
            get { return _childTemplateId; }
            set
            {
                if (_childTemplateId == value)
                    return;

                _childTemplateId = value;

                // since the template has changed then let's force the list to redisplay by firing NotifyDataSetChanged()
                if (ItemCount > 0)
                    NotifyDataSetChanged();
            }
        }
        private int _childTemplateId;

        public IExpandCollapseListener ExpandCollapseListener { get; private set; }

        private readonly List<object> FlatennedItemsSource = new List<object>();

        private readonly List<RecyclerView> AttachedRecyclerViewPool = new List<RecyclerView>();

        public override int ItemCount => FlatennedItemsSource.Count;


        public virtual IList SelectedItems
        {
            get { return _selectedItems; }
            set { SetSelectedItems(value); }
        }
        private IList _selectedItems;

        public virtual bool SingleSelection
        {
            get { return _singleSelection; }
            set { SetSingleSelection(value); }
        }
        private bool _singleSelection;

        public virtual bool SelectionEnabled
        {
            get { return _selectionEnabled; }
            set { SetSelectionEnabled(value); }
        }
        private bool _selectionEnabled;

        public virtual bool SingleExpansion
        {
            get { return _singleExpansion; }
            set { SetSingleExpansion(value); }
        }        
        private bool _singleExpansion;

        #endregion

        #region Commands

        protected virtual ICommand MyItemClick
        {
            get { return _myItemClick ?? (_myItemClick = new MvxCommand<object>(OnItemClick)); }
        }
        private ICommand _myItemClick;

        #endregion

        #region Constructor

        public BaseExpandableRecyclerViewAdapter()
            : this(MvxAndroidBindingContextHelpers.Current())
        {
        }

        public BaseExpandableRecyclerViewAdapter(IMvxAndroidBindingContext bindingContext)
            : base(bindingContext)
        {
        }

        #endregion

        #region IParentExpandCollapseListener Implementation

        /// <summary>
        /// Implementation of <see cref="IParentExpandCollapseListener.OnParentCollapse(PVH)"/>.
        /// Called when a <see cref="IGroupItem"/> is triggered to expand.
        /// </summary>
        /// <param name="position">The index of the item in the list being expanded</param>
        public void OnParentExpand(ParentViewHolder parentViewHolder)
        {
            var groupItem = parentViewHolder.DataContext as IGroupItem;
            if (groupItem != null)
                ExpandParent(groupItem, parentViewHolder.AdapterPosition, true);
        }

        /// <summary>
        /// Implementation of <see cref="IParentExpandCollapseListener.OnParentCollapse(int)"/>.
        /// Called when a <see cref="IGroupItem"/> is triggered to collapse.
        /// </summary>
        /// <param name="position">The index of the item in the list being collapsed</param>
        public void OnParentCollapse(ParentViewHolder parentViewHolder)
        {
            var groupItem = parentViewHolder.DataContext as IGroupItem;
            if (groupItem != null)
                CollapseViews(groupItem, parentViewHolder.AdapterPosition, true);
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Callback called from <see cref="OnCreateViewHolder(ViewGroup, int)"/> when
        /// the list item created is a parent.
        /// </summary>
        /// <param name="parentViewGroup">The <see cref="ViewGroup"/> in the list for which a <see cref="PVH"/> is being created</param>
        /// <returns>A <see cref="PVH"/> corresponding to the parent item with the <see cref="ViewGroup"/> parentViewGroup</returns>
        public abstract PVH OnCreateParentViewHolder(ViewGroup parentViewGroup);

        /// <summary>
        /// Callback called from <see cref="OnCreateViewHolder(ViewGroup, int)"/> when
        /// the list item created is a child.
        /// </summary>
        /// <param name="childViewGroup">The <see cref="ViewGroup"/> in the list for which a <see cref="CVH"/> is being created</param>
        /// <returns>A <see cref="CVH"/> corresponding to the child item with the <see cref="ViewGroup"/> childViewGroup</returns>
        public abstract CVH OnCreateChildViewHolder(ViewGroup childViewGroup);

        /// <summary>
        /// Callback called from <see cref="OnBindViewHolder(RecyclerView.ViewHolder, int)"/>
        /// when the list item bound to is a parent.
        /// </summary>
        /// <param name="parentViewHolder">The <see cref="PVH"/> to bind data to</param>
        /// <param name="position">The index in the list at which to bind</param>
        /// <param name="parentListItem">The <see cref="IGroupItem"/> item which holds the data to be bound to the <see cref="PVH"/> </param>
        public abstract void OnBindParentViewHolder(PVH parentViewHolder, int position, IGroupItem parentListItem);

        /// <summary>
        /// Callback called from <see cref="OnBindViewHolder(RecyclerView.ViewHolder, int)"/>
        /// when the list item bound to is a child.
        /// </summary>
        /// <param name="childViewHolder">The <see cref="CVH"/> to bind data to</param>
        /// <param name="position">The index in the list at which to bind</param>
        /// <param name="childListItem">The child item which holds the data to be bound to the <see cref="CVH"/> </param>
        public abstract void OnBindChildViewHolder(CVH childViewHolder, int position, object childListItem);

        #endregion

        #region Methods
        

        /// <summary>
        /// Gets the index of a <see cref="IGroupItem"/> within the flatenned items source based on
        /// the index of the <see cref="IGroupItem"/>.
        /// </summary>
        /// <param name="parentIndex">The index of the parent in the list of parent items.</param>
        /// <returns>The index of the parent in the list of all views in the adapter</returns>
        private int GetGroupItemIndex(int parentIndex)
        {
            int parentCount = 0;
            for (int i = 0; i < ItemCount; i++)
            {
                if (typeof(IGroupItem).IsAssignableFrom(FlatennedItemsSource[i].GetType()))
                {
                    parentCount++;

                    if (parentCount > parentIndex)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the item within the flatenned items source.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override object GetItem(int position)
        {
            return FlatennedItemsSource.ElementAt(position);
        }

        /// <summary>
        /// Gets the index of the item within the flatenned items source.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public virtual int GetPosition(object item)
        {
            return FlatennedItemsSource.GetPosition(item);
        }


        protected virtual void SetItemSelection(object item, bool value)
        {
            _selectableViewHolderManager.SetItemSelection(item, value);
        }

        protected virtual void SetAllItemsSelection(bool value)
        {
            _selectableViewHolderManager.SetAllItemsSelection(value);
        }

        protected virtual void OnItemClick(object clickedItem)
        {
            if (clickedItem == null)
                return;

            try
            {
                if (SelectionEnabled)
                {
                    var isSelected = _selectableViewHolderManager.IsItemSelected(clickedItem);
                    if (isSelected)
                    {
                        //remove it from the selected items list
                        if (SelectedItems.Contains(clickedItem))
                        {
                            var canRemove = (ItemClick == null || ItemClick.CanExecute(clickedItem));
                            if (canRemove)
                                SelectedItems.Remove(clickedItem);

                            if (ItemClick != null)
                                ItemClick.Execute(clickedItem);
                        }
                    }
                    else
                    {
                        var canSelectItem = true;
                        if (ItemClick != null)
                            canSelectItem = ItemClick.CanExecute(clickedItem);

                        if (canSelectItem)
                        {
                            if (!SelectedItems.Contains(clickedItem))
                            {
                                if (SingleSelection)
                                    SelectedItems.Clear();
                                SelectedItems.Add(clickedItem);

                                if (ItemClick != null)
                                    ItemClick.Execute(clickedItem);
                            }
                        }
                        //else
                        //{
                        //    //If the item can't be added to the selected items,
                        //    //we must set the correspondent view to 'uncheked' state
                        //    this.SetItemChecked(e.Position, false);
                        //}
                    }
                }
                else
                {
                    if (ItemClick != null && ItemClick.CanExecute(clickedItem))
                        ItemClick.Execute(clickedItem);
                }
            }
            catch (System.Exception ex)
            {
                Android.Util.Log.Wtf("SelectableRecyclerView.OnItemClicked", ex.Message);
            }
        }


        protected virtual void SetSingleExpansion(bool value)
        {
            if (_singleExpansion != value)
            {
                _singleExpansion = value;
                SetSingleExpansion();
            }
        }
        private void SetSingleExpansion()
        {
            if (SingleExpansion)
                CollapseAllParentsButFirst();
        }

        protected virtual void SetSelectionEnabled(bool value)
        {
            if (_selectionEnabled != value)
            {
                _selectionEnabled = value;
                SetSelectionEnabled();
            }
        }
        private void SetSelectionEnabled()
        {
            _selectableViewHolderManager.SetSelectionEnabled(SelectionEnabled);
        }

        protected virtual void SetSingleSelection(bool value, bool forceVisualRefresh = false)
        {
            if (_singleSelection != value)
            {
                _singleSelection = value;
                if (_singleSelection && SelectedItems.SafeCount() > 1)
                    SelectedItems.SafeClear();

                SetSingleSelection();
            }
            else if (forceVisualRefresh)
            {
                SetSingleSelection();
            }
        }
        protected virtual void SetSingleSelection()
        {
            _selectableViewHolderManager.SetSingleSelection(SingleSelection);
        }

        public void SetSelectedItems(IList items)
        {
            if (_selectedItemscollectionChangedToken != null)
            {
                _selectedItemscollectionChangedToken.Dispose();
                _selectedItemscollectionChangedToken = null;
            }

            if (items == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to BaseExpandableRecyclerViewAdapter.SelectedItems binding");
                return;
            }

            if (typeof(INotifyCollectionChanged).IsAssignableFrom(items.GetType()))
            {
                _selectedItemscollectionChangedToken = (items as INotifyCollectionChanged).WeakSubscribe(OnSelectedItemsCollectionChanged);
            }

            _selectedItems = items;
            if (_selectedItems.Count > 0)
            {
                foreach (var item in _selectedItems)
                {
                    SetItemSelection(item, true);
                }
            }
            else
            {
                SetAllItemsSelection(false);
            }
        }
        private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in e.NewItems)
                        {
                            SetItemSelection(item, true);
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                        Android.Util.Log.Error("BaseExpandableRecyclerViewAdapter.OnSelectedItemsCollectionChanged", "NotifyCollectionChangedAction.Move not supported");
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in e.OldItems)
                        {
                            SetItemSelection(item, false);
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        Android.Util.Log.Error("BaseExpandableRecyclerViewAdapter.OnSelectedItemsCollectionChanged", "NotifyCollectionChangedAction.Replace not supported");
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        SetAllItemsSelection(false);
                        break;

                    default:
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Android.Util.Log.Wtf("BaseExpandableRecyclerViewAdapter.OnSelectedItemsCollectionChanged", ex.Message);
            }
        }
        
        protected override void SetItemsSource(IEnumerable value)
        {
            _selectableViewHolderManager.SetItemsSource(value, SelectedItems);

            base.SetItemsSource(value);

            FlattenItemsSource();
        }
        protected override void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            try
            {
                if (args != null)
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Move:
                            Android.Util.Log.Error("SelectableRecyclerViewAdapter.OnItemsSourceChanged", "NotifyCollectionChangedAction.Move not supported");
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            Android.Util.Log.Error("SelectableRecyclerViewAdapter.OnItemsSourceChanged", "NotifyCollectionChangedAction.Replace not supported");
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            if (SelectedItems != null)
                            {
                                foreach (IGroupItem groupItem in args.OldItems)
                                {
                                    if (groupItem == null)
                                        throw new InvalidCastException("Items source must be of type IGroupItem");

                                    if (groupItem.Children.SafeCount() > 0)
                                    {
                                        foreach (var itemChild in groupItem.Children)
                                        {
                                            if (SelectedItems.Contains(itemChild))
                                                SelectedItems.Remove(itemChild);
                                        }
                                    }
                                }
                            }
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            SelectedItems.SafeClear();
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Android.Util.Log.Wtf("SelectableRecyclerView.OnItemsSourceChanged", ex.Message);
            }

            _selectableViewHolderManager.OnItemsSourceCollectionChanged(args);

            FlattenItemsSource();

            NotifyDataSetChanged();
        }
        private void OnGroupItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var groupItem = sender as IGroupItem;
            if (groupItem != null && e.PropertyName == nameof(groupItem.IsExpanded))
            {
                if (groupItem.IsExpanded)
                    ExpandParent(groupItem, false);
                else
                    CollapseParent(groupItem, false);
            }
        }
        private void OnItemsSourceChildCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            try
            {
                if (args != null)
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Move:
                            Android.Util.Log.Error("SelectableRecyclerViewAdapter.OnItemsSourceChildChanged", "NotifyCollectionChangedAction.Move not supported");
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            Android.Util.Log.Error("SelectableRecyclerViewAdapter.OnItemsSourceChildChanged", "NotifyCollectionChangedAction.Replace not supported");
                            break;


                        case NotifyCollectionChangedAction.Reset:
                        case NotifyCollectionChangedAction.Remove:
                            if (SelectedItems != null)
                            {
                                foreach (var itemChild in args.OldItems)
                                {
                                    if (SelectedItems.Contains(itemChild))
                                        SelectedItems.Remove(itemChild);
                                }
                            }
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Android.Util.Log.Wtf("SelectableRecyclerView.OnItemsSourceChildChanged", ex.Message);
            }

            _selectableViewHolderManager.OnItemsSourceChildCollectionChanged(args);

            FlattenItemsSource();

            NotifyDataSetChanged();
        }

        private void FlattenItemsSource()
        {
            FlatennedItemsSource.Clear();

            _propertyChangedSubscriptions.ForEach(s => s.Dispose());
            _propertyChangedSubscriptions.Clear();

            _itemsSourceChildCollectionChangedTokenList.SafeForEach(s => s.Dispose());
            _itemsSourceChildCollectionChangedTokenList.Clear();

            if (ItemsSource.SafeCount() > 0)
            {
                foreach (IGroupItem groupItem in ItemsSource)
                {
                    if (groupItem == null)
                        throw new InvalidCastException("ItemsSource must be a list of IGroupItem");

                    var groupItemNotifiable = groupItem as INotifyPropertyChanged;
                    if (groupItemNotifiable != null)
                        _propertyChangedSubscriptions.Add(groupItemNotifiable.WeakSubscribe(OnGroupItemPropertyChanged));

                    var newObservable = groupItem.Children as INotifyCollectionChanged;
                    if (newObservable != null)
                        _itemsSourceChildCollectionChangedTokenList.Add(newObservable.WeakSubscribe(OnItemsSourceChildCollectionChanged));

                    FlatennedItemsSource.Add(groupItem);

                    if (groupItem.IsInitiallyExpanded || groupItem.IsExpanded)
                    {
                        if (groupItem.Children.SafeCount() > 0)
                        {
                            foreach (var itemChild in groupItem.Children)
                            {
                                FlatennedItemsSource.Add(itemChild);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Implementation of Adapter.onCreateViewHolder(ViewGroup, int)
        /// that determines if the list item is a parent or a child and calls through
        ///to the appropriate implementation of either 
        ///<see cref="onCreateParentViewHolder(ViewGroup)"/> or <see cref="onCreateChildViewHolder(ViewGroup)"/>
        /// </summary>
        /// <param name="parent">The <see cref="ViewGroup"/> into which the new <see cref="View"/> will be added after it is bound to an adapter position.</param>
        /// <param name="viewType">The view type of the new <see cref="View"/>.</param>
        /// <returns>A new RecyclerView.ViewHolder that holds a <see cref="View"/> of the given view type.</returns>
        /// <exception cref="Java.Lang.IllegalStateException">Incorrect ViewType found</exception>
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == TYPE_PARENT)
            {
                PVH pvh = OnCreateParentViewHolder(parent);
                pvh.SetExpandCollapseListener(this);
                return pvh;
            }
            else if (viewType == TYPE_CHILD)
            {
                var cvh = OnCreateChildViewHolder(parent);
                cvh.Click = MyItemClick;
                cvh.LongClick = this.ItemLongClick;
                return cvh;
            }
            else
            {
                throw new IllegalStateException("Incorrect ViewType found");
            }
        }

        /// <summary>
        /// Implementation of Adapter.OnBindViewHolder(RecyclerView.ViewHolder, int)
        /// that determines if the list item is a parent or a child and calls through
        /// to the appropriate implementation of either <see cref="OnBindParentViewHolder(PVH, int, IGroupItem)"/>
        /// or <see cref="OnBindChildViewHolder(CVH, int, object)"/>.
        /// </summary>
        /// <param name="holder">The RecyclerView.ViewHolder to bind data to</param>
        /// <param name="position">The index in the list at which to bind</param>
        /// <exception cref="Java.Lang.IllegalStateException">Incorrect ViewHolder found</exception>
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var rawItem = GetItem(position);
            if (rawItem == null)
                throw new IllegalStateException("Incorrect ViewHolder found");

            if (typeof(IGroupItem).IsAssignableFrom(rawItem.GetType()))
            {
                PVH parentViewHolder = (PVH)holder;

                if (parentViewHolder.ItemViewClickTogglesExpansion)
                    parentViewHolder.SetMainItemClickToExpand();

                var groupItem = rawItem as IGroupItem;
                OnBindParentViewHolder(parentViewHolder, position, groupItem);
            }
            else
            {
                OnBindChildViewHolder((CVH)holder, position, rawItem);                            
                _selectableViewHolderManager.BindHolder(holder as SelectableViewHolder, rawItem, position, SingleSelection, SelectionEnabled);
            }
        }

        /// <summary>
        /// Gets the view type of the item at the given position.
        /// </summary>
        /// <param name="position">The index in the list to get the view type of</param>
        /// <returns><see cref="TYPE_PARENT"/> for <see cref="IGroupItem"/> and <see cref="TYPE_CHILD"/> for child list items</returns>
        /// <exception cref="Java.Lang.IllegalStateException">Null object added</exception>
        public override int GetItemViewType(int position)
        {
            var rawItem = GetItem(position);
            if (rawItem == null)
                throw new IllegalStateException("GetItemViewType -> Item is Null");

            if (typeof(IGroupItem).IsAssignableFrom(rawItem.GetType()))
                return TYPE_PARENT;

            return TYPE_CHILD;
        }

        /// <summary>
        /// Called when [view attached to window].
        /// </summary>
        /// <param name="holder">The holder.</param>
        public override void OnViewAttachedToWindow(Java.Lang.Object holder)
        {
            base.OnViewAttachedToWindow(holder);

            if (typeof(PVH).IsAssignableFrom(holder.GetType()))
            {
                var parentViewHolder = ((PVH)holder);
                if (parentViewHolder.DataContext == null)
                {
                    BindViewHolder(parentViewHolder, parentViewHolder.AdapterPosition);
                }
            }
        }

        
        /// <summary>
        /// Called when this <see cref="BaseExpandableRecyclerViewAdapter{PVH, CVH}"/> is attached to a RecyclerView.
        /// </summary>
        /// <param name="recyclerView">The <see cref="RecyclerView"/> this <see cref="BaseExpandableRecyclerViewAdapter{PVH, CVH}"/> is being attached to</param>
        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);

            AttachedRecyclerViewPool.Add(recyclerView);
        }

        /// <summary>
        /// Called when this <see cref="BaseExpandableRecyclerViewAdapter{PVH, CVH}"/> is detached to a RecyclerView.
        /// </summary>
        /// <param name="recyclerView">The <see cref="RecyclerView"/> this <see cref="BaseExpandableRecyclerViewAdapter{PVH, CVH}"/> is being detached from</param>
        public override void OnDetachedFromRecyclerView(RecyclerView recyclerView)
        {
            base.OnDetachedFromRecyclerView(recyclerView);

            AttachedRecyclerViewPool.Remove(recyclerView);
        }

        #endregion

        #region Programmatic Expansion/Collapsing

        /// <summary>
        /// Expands the parent with the specified index in the list of parents.
        /// </summary>
        /// <param name="groupItemIndex">The index of the parent to expand</param>
        public void ExpandParent(int parentIndex, bool expansionTriggeredByListItemClick)
        {
            if (!_isExpanding)
            {
                int groupItemIndex = GetGroupItemIndex(parentIndex);
                var groupItem = GetItem(groupItemIndex) as IGroupItem;

                if (groupItem != null)
                    ExpandParent(groupItem, groupItemIndex, expansionTriggeredByListItemClick);
            }
        }

        /// <summary>
        /// Expands the parent associated with a specified <see cref="IGroupItem"/> in
        /// the list of parents.
        /// </summary>
        /// <param name="groupItem">The <see cref="IGroupItem"/> to expand</param>
        public void ExpandParent(IGroupItem groupItem, bool expansionTriggeredByListItemClick)
        {
            if (!_isExpanding)
            {
                var groupItemIndex = GetPosition(groupItem);
                if (groupItemIndex > -1)
                    ExpandParent(groupItem, groupItemIndex, expansionTriggeredByListItemClick);
            }
        }

        /// <summary>
        /// Expands the parent associated with a specified <see cref="IGroupItem"/> in
        /// the list of parents and collapses the other parents if <see cref="SingleExpansion"/> is <c>true</c>
        /// </summary>
        /// <param name="groupItem">The group item.</param>
        /// <param name="groupItemIndex">Index of the group item.</param>
        /// <param name="expansionTriggeredByListItemClick">if set to <c>true</c> the expansion triggered by list item click.</param>
        /// <remarks>
        /// If <see cref="SingleExpansion"/> is <c>true</c>, the other parents are collapsed before expanding <paramref name="groupItem"/>
        /// </remarks>
        private void ExpandParent(IGroupItem groupItem, int groupItemIndex, bool expansionTriggeredByListItemClick)
        {
            //If the adapter is configured for single expansion, all other expanded items are collapsed
            if (SingleExpansion)
            {
                CollapseAllButCurrent(groupItem);

                //As other items were collapsed, the current groupItemIndex is no longer correct. The position of the groupItem is obtained again.
                groupItemIndex = GetPosition(groupItem);
            }

            if(groupItemIndex > -1)
                ExpandViews(groupItem, groupItemIndex, expansionTriggeredByListItemClick);
        }

        /// <summary>
        /// Expands all parents in a range of indices in the list of parents.
        /// </summary>
        /// <param name="startParentIndex">The index at which to to start expanding parents</param>
        /// <param name="parentCount">The number of parents to expand</param>
        public void ExpandParentRange(int startParentIndex, int parentCount)
        {
            int endParentIndex = startParentIndex + parentCount;
            for (int i = startParentIndex; i < endParentIndex; i++)
            {
                ExpandParent(i, false);
            }
        }

        /// <summary>
        /// Expands all parents in the list.
        /// </summary>
        public void ExpandAllParents()
        {
            foreach (IGroupItem groupItem in ItemsSource)
            {
                if (groupItem == null)
                    throw new InvalidCastException("ItemsSource must be a list of IGroupItem");

                ExpandParent(groupItem, false);
            }
        }


        /// <summary>
        /// Calls through to the ParentViewHolder to expand views for each
        /// RecyclerView the specified parent is a child of.
        /// These calls to the ParentViewHolder are made so that animations can be
        /// triggered at the ViewHolder level.
        /// </summary>
        /// <param name="parentGroupItem">The parent group item to expand</param>
        /// <param name="parentIndex">The index of the parent to expand.</param>
        private void ExpandViews(IGroupItem parentGroupItem, int parentIndex, bool expansionTriggeredByListItemClick)
        {
            if (!_isExpanding)
            {                
                //PVH viewHolder;
                foreach (var recyclerView in AttachedRecyclerViewPool)
                {
                    //viewHolder = (PVH)recyclerView.FindViewHolderForAdapterPosition(parentIndex);
                    //if (viewHolder != null && !viewHolder.IsExpanded)
                    //{
                    //    viewHolder.IsExpanded = true;
                    //    viewHolder.OnExpansionToggled(false);
                    //}

                    CommonExpandParent(recyclerView, parentGroupItem, parentIndex, expansionTriggeredByListItemClick);
                }
            }
        }

        /// <summary>
        /// Expands a specified parent item. Calls through to the
        /// ExpandCollapseListener and adds children of the specified parent to the
        /// total list of items.
        /// </summary>
        /// <param name="parentGroupItem">The ParentWrapper of the parent to expand</param>
        /// <param name="parentIndex">The index of the parent to expand</param>
        /// <param name="expansionTriggeredByListItemClick">if expansion was triggered by a click event, false otherwise.</param>
        private void CommonExpandParent(RecyclerView recyclerView, IGroupItem parentGroupItem, int parentIndex, bool expansionTriggeredByListItemClick)
        {
            if (!_isExpanding)
            {
                try
                {
                    var canContinue = (expansionTriggeredByListItemClick && !parentGroupItem.IsExpanded)
                                      ||
                                      (!expansionTriggeredByListItemClick && parentGroupItem.IsExpanded);
                    if (canContinue)
                    {
                        _isExpanding = true;

                        if (expansionTriggeredByListItemClick)
                        {
                            var viewHolder = (PVH)recyclerView.FindViewHolderForAdapterPosition(parentIndex);
                            if (viewHolder != null)
                                viewHolder.SetIsExpanded(true);
                        }

                        var childItemList = parentGroupItem.Children;
                        if (childItemList != null)
                        {
                            int childListItemCount = childItemList.Count();
                            bool useAdd = ((parentIndex + 1) >= ItemCount);
                            for (int i = 0; i < childListItemCount; i++)
                            {
                                if (useAdd)
                                    FlatennedItemsSource.Add(childItemList.ElementAt(i));
                                else
                                    FlatennedItemsSource.Insert(parentIndex + i + 1, childItemList.ElementAt(i));
                            }

                            NotifyItemRangeInserted(parentIndex + 1, childListItemCount);
                        }

                        if (expansionTriggeredByListItemClick && ExpandCollapseListener != null)
                        {
                            int expandedCountBeforePosition = GetExpandedItemCount(parentIndex);
                            ExpandCollapseListener.OnListItemExpanded(parentIndex - expandedCountBeforePosition);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    var exe = ex;
                    if (Debugger.IsAttached)
                        Debugger.Break();
                }
                finally
                {
                    _isExpanding = false;
                }
            }
        }



        /// <summary>
        /// Collapses the parent with the specified index in the list of parents.
        /// </summary>
        /// <param name="parentIndex">The index of the parent to collapse</param>
        public void CollapseParent(int parentIndex, bool collapseTriggeredByListItemClick)
        {
            if (!_isCollapsing)
            {
                int groupItemIndex = GetGroupItemIndex(parentIndex);
                var groupItem = GetItem(groupItemIndex) as IGroupItem;

                if (groupItem != null)
                    CollapseViews(groupItem, groupItemIndex, collapseTriggeredByListItemClick);
            }
        }

        /// <summary>
        /// Collapses the parent associated with a specified <see cref="IGroupItem"/> in
        /// the list of parents.
        /// </summary>
        /// <param name="groupItem">The <see cref="IGroupItem"/> to collapse.</param>
        public void CollapseParent(IGroupItem groupItem, bool collapseTriggeredByListItemClick)
        {
            if (!_isCollapsing)
            {
                var groupItemIndex = GetPosition(groupItem);
                if (groupItemIndex > -1)
                    CollapseViews(groupItem, groupItemIndex, collapseTriggeredByListItemClick);
            }
        }

        /// <summary>
        /// Collapses all parents in a range of indices in the list of parents.
        /// </summary>
        /// <param name="startParentIndex">The index at which to to start collapsing parents</param>
        /// <param name="parentCount">The number of parents to collapse</param>
        public void CollapseParentRange(int startParentIndex, int parentCount)
        {
            int endParentIndex = startParentIndex + parentCount;
            for (int i = startParentIndex; i < endParentIndex; i++)
            {
                CollapseParent(i, false);
            }
        }

        /// <summary>
        /// Collapses all parents in the list.
        /// </summary>
        public void CollapseAllParents()
        {
            foreach (IGroupItem groupItem in ItemsSource)
            {
                if (groupItem == null)
                    throw new InvalidCastException("ItemsSource must be a list of IGroupItem");

                groupItem.IsExpanded = false;
            }
        }

        protected virtual void CollapseAllButCurrent(IGroupItem currentGroupItem)
        {
            foreach (IGroupItem item in ItemsSource)
            {
                if (item != null && !item.Equals(currentGroupItem) && item.IsExpanded)
                    item.IsExpanded = false;
            }
        }

        protected virtual void CollapseAllParentsButFirst()
        {
            var foundFirst = false;
            foreach (IGroupItem groupItem in ItemsSource)
            {
                if (groupItem == null)
                    throw new InvalidCastException("ItemsSource must be a list of IGroupItem");

                if(groupItem.IsExpanded)
                {
                    if (!foundFirst)
                        foundFirst = true;
                    else
                        groupItem.IsExpanded = false;
                }
            }
        }


        /// <summary>
        /// Calls through to the ParentViewHolder to collapse views for each
        /// RecyclerView the specified parent is a child of.
        /// These calls to the ParentViewHolder are made so that animations can be
        /// triggered at the ViewHolder level.
        /// </summary>
        /// <param name="parentGroupItem">The parent group item to collapse</param>
        /// <param name="parentIndex">The index of the parent to collapse.</param>
        private void CollapseViews(IGroupItem parentGroupItem, int parentIndex, bool collapseTriggeredByListItemClick)
        {
            if (!_isCollapsing)
            {
                //PVH viewHolder;
                foreach (var recyclerView in AttachedRecyclerViewPool)
                {
                    //viewHolder = (PVH)recyclerView.FindViewHolderForAdapterPosition(parentIndex);
                    //if (viewHolder != null && viewHolder.IsExpanded)
                    //{
                    //    viewHolder.IsExpanded = false;
                    //    viewHolder.OnExpansionToggled(true);
                    //}

                    CommonCollapseParent(recyclerView, parentGroupItem, parentIndex, collapseTriggeredByListItemClick);
                }
            }
        }
        /// <summary>
        /// Collapses a specified parent item. Calls through to the
        /// ExpandCollapseListener and adds children of the specified parent to the
        /// total list of items.
        /// </summary>
        /// <param name="parentGroupItem">The ParentWrapper of the parent to collapse.</param>
        /// <param name="parentIndex">The index of the parent to collapse.</param>
        /// <param name="collapseTriggeredByListItemClick">true if expansion was triggered by a click event, false otherwise.</param>
        private void CommonCollapseParent(RecyclerView recyclerView, IGroupItem parentGroupItem, int parentIndex, bool collapseTriggeredByListItemClick)
        {
            if (!_isCollapsing)
            {
                try
                {
                    var canContinue = (collapseTriggeredByListItemClick && parentGroupItem.IsExpanded)
                                      ||
                                      (!collapseTriggeredByListItemClick && !parentGroupItem.IsExpanded);
                    if (canContinue)
                    {
                        _isCollapsing = true;

                        if (collapseTriggeredByListItemClick)
                        {
                            var viewHolder = (PVH)recyclerView.FindViewHolderForAdapterPosition(parentIndex);
                            if (viewHolder != null)
                                viewHolder.SetIsExpanded(false);
                        }

                        var childItemList = parentGroupItem.Children;
                        if (childItemList != null)
                        {
                            int childListItemCount = childItemList.Count();
                            FlatennedItemsSource.RemoveRange(parentIndex + 1, childListItemCount);
                            NotifyItemRangeRemoved(parentIndex + 1, childListItemCount);
                        }

                        if (collapseTriggeredByListItemClick && ExpandCollapseListener != null)
                        {
                            int expandedCountBeforePosition = GetExpandedItemCount(parentIndex);
                            ExpandCollapseListener.OnListItemCollapsed(parentIndex - expandedCountBeforePosition);
                        }
                    }
                }
                finally
                {
                    _isCollapsing = false;
                }
            }
        }



        /// <summary>
        /// Gets the number of expanded child list items before the specified position.
        /// </summary>
        /// <param name="position">The index before which to return the number of expanded child list items</param>
        /// <returns>The number of expanded child list items before the specified position</returns>
        private int GetExpandedItemCount(int position)
        {
            if (position == 0)
                return 0;

            int expandedCount = 0;
            for (int i = 0; i < position; i++)
            {
                var groupItem = GetItem(i) as IGroupItem;
                if (groupItem == null)
                    expandedCount++;
            }

            return expandedCount;
        }

        #endregion



        #region Review Restore and Save State

        ///// <summary>
        ///// Stores the expanded state map across state loss.
        ///// hould be called from OnSaveInstanceState in
        ///// the Activity that hosts the RecyclerView that this
        ///// <see cref="ExpandableRecyclerViewAdapter{PVH, CVH}"/> is attached to.
        ///// This will make sure to add the expanded state map as an extra to the
        ///// instance state bundle to be used in <see cref="OnRestoreInstanceState(Bundle)"/>.
        ///// </summary>
        ///// <param name="savedInstanceState">The {@code Bundle} into which to store the expanded state map</param>
        //public void onSaveInstanceState(Bundle savedInstanceState)
        //{
        //    savedInstanceState.PutSerializable(EXPANDED_STATE_MAP, GenerateExpandedStateMap());
        //}

        ///// <summary>
        ///// Fetches the expandable state map from the saved instance state <see cref="Bundle"/>
        ///// Should be called from OnRestoreInstanceState in
        ///// the Activity that hosts the RecyclerView that this
        ///// <see cref="ExpandableRecyclerViewAdapter{PVH, CVH}"/> is attached to.
        ///// Assumes that the list of parent list items is the same as when the saved
        ///// instance state was stored..
        ///// </summary>
        ///// <param name="savedInstanceState">The {@code Bundle} from which the expanded state map is loaded</param>
        //public void OnRestoreInstanceState(Bundle savedInstanceState)
        //{
        //    if (savedInstanceState == null
        //            || !savedInstanceState.ContainsKey(EXPANDED_STATE_MAP))
        //    {
        //        return;
        //    }

        //    var expandedStateMap = (HashMap)savedInstanceState.GetSerializable(EXPANDED_STATE_MAP);
        //    if (expandedStateMap == null)
        //    {
        //        return;
        //    }

        //    List<Object> parentWrapperList = new ArrayList<>();
        //    ParentListItem parentListItem;
        //    ParentWrapper parentWrapper;

        //    int parentListItemCount = mParentItemList.size();
        //    for (int i = 0; i < parentListItemCount; i++)
        //    {
        //        parentListItem = mParentItemList.get(i);
        //        parentWrapper = new ParentWrapper(parentListItem);
        //        parentWrapperList.add(parentWrapper);

        //        if (expandedStateMap.containsKey(i))
        //        {
        //            boolean expanded = expandedStateMap.get(i);
        //            if (expanded)
        //            {
        //                parentWrapper.setExpanded(true);

        //                int childListItemCount = parentWrapper.getChildItemList().size();
        //                for (int j = 0; j < childListItemCount; j++)
        //                {
        //                    parentWrapperList.add(parentWrapper.getChildItemList().get(j));
        //                }
        //            }
        //        }
        //    }

        //    mItemList = parentWrapperList;

        //    notifyDataSetChanged();
        //}

        //    /// <summary>
        //    /// Generates a HashMap used to store expanded state for items in the list
        //    /// on configuration change or whenever onResume is called.
        //    /// </summary>
        //    /// <returns>A HashMap containing the expanded state of all parent list items</returns>
        //    private HashMap GenerateExpandedStateMap()
        //    {
        //        HashMap parentListItemHashMap = new HashMap();
        //        int childCount = 0;

        //        Object listItem;
        //        ParentWrapper parentWrapper;
        //        int listItemCount = mItemList.size();
        //        for (int i = 0; i < listItemCount; i++)
        //        {
        //            if (mItemList.get(i) != null)
        //            {
        //                listItem = getListItem(i);
        //                if (listItem instanceof ParentWrapper) 
        //                {
        //            parentWrapper = (ParentWrapper)listItem;
        //            parentListItemHashMap.put(i - childCount, parentWrapper.isExpanded());
        //        } 
        //                else 
        //                {
        //            childCount++;
        //        }
        //    }
        //}

        //        return parentListItemHashMap;
        //    }

        #endregion

        #region Review Data Manipulation

        ///**
        //    * Notify any registered observers that the ParentListItem reflected at {@code parentPosition}
        //    * has been newly inserted. The ParentListItem previously at {@code parentPosition} is now at
        //    * position {@code parentPosition + 1}.
        //    * <p>
        //    * This is a structural change event. Representations of other existing items in the
        //    * data set are still considered up to date and will not be rebound, though their
        //    * positions may be altered.
        //    *
        //    * @param parentPosition Position of the newly inserted ParentListItem in the data set, relative
        //    *                       to list of ParentListItems only.
        //    *
        //    * @see #notifyParentItemRangeInserted(int, int)
        //    */
        //public void notifyParentItemInserted(int parentPosition)
        //{
        //    ParentListItem parentListItem = mParentItemList.get(parentPosition);

        //    int wrapperIndex;
        //    if (parentPosition < mParentItemList.size() - 1)
        //    {
        //        wrapperIndex = getParentWrapperIndex(parentPosition);
        //    }
        //    else {
        //        wrapperIndex = mItemList.size();
        //    }

        //    int sizeChanged = addParentWrapper(wrapperIndex, parentListItem);
        //    notifyItemRangeInserted(wrapperIndex, sizeChanged);
        //}

        ///**
        //    * Notify any registered observers that the currently reflected {@code itemCount}
        //    * ParentListItems starting at {@code parentPositionStart} have been newly inserted.
        //    * The ParentListItems previously located at {@code parentPositionStart} and beyond
        //    * can now be found starting at position {@code parentPositionStart + itemCount}.
        //    * <p>
        //    * This is a structural change event. Representations of other existing items in the
        //    * data set are still considered up to date and will not be rebound, though their positions
        //    * may be altered.
        //    *
        //    * @param parentPositionStart Position of the first ParentListItem that was inserted, relative
        //    *                            to list of ParentListItems only.
        //    * @param itemCount Number of items inserted
        //    *
        //    * @see #notifyParentItemInserted(int)
        //    */
        //public void notifyParentItemRangeInserted(int parentPositionStart, int itemCount)
        //{
        //    int initialWrapperIndex;
        //    if (parentPositionStart < mParentItemList.size() - itemCount)
        //    {
        //        initialWrapperIndex = getParentWrapperIndex(parentPositionStart);
        //    }
        //    else {
        //        initialWrapperIndex = mItemList.size();
        //    }

        //    int sizeChanged = 0;
        //    int wrapperIndex = initialWrapperIndex;
        //    int changed;
        //    int parentPositionEnd = parentPositionStart + itemCount;
        //    for (int i = parentPositionStart; i < parentPositionEnd; i++)
        //    {
        //        ParentListItem parentListItem = mParentItemList.get(i);
        //        changed = addParentWrapper(wrapperIndex, parentListItem);
        //        wrapperIndex += changed;
        //        sizeChanged += changed;
        //    }

        //    notifyItemRangeInserted(initialWrapperIndex, sizeChanged);
        //}

        //private int addParentWrapper(int wrapperIndex, ParentListItem parentListItem)
        //{
        //    int sizeChanged = 1;
        //    ParentWrapper parentWrapper = new ParentWrapper(parentListItem);
        //    mItemList.add(wrapperIndex, parentWrapper);
        //    if (parentWrapper.isInitiallyExpanded())
        //    {
        //        parentWrapper.setExpanded(true);
        //        List <?> childItemList = parentWrapper.getChildItemList();
        //        mItemList.addAll(wrapperIndex + sizeChanged, childItemList);
        //        sizeChanged += childItemList.size();
        //    }
        //    return sizeChanged;
        //}

        ///**
        //    * Notify any registered observers that the ParentListItem previously located at {@code parentPosition}
        //    * has been removed from the data set. The ParentListItems previously located at and after
        //    * {@code parentPosition} may now be found at {@code oldPosition - 1}.
        //    * <p>
        //    * This is a structural change event. Representations of other existing items in the
        //    * data set are still considered up to date and will not be rebound, though their positions
        //    * may be altered.
        //    *
        //    * @param parentPosition Position of the ParentListItem that has now been removed, relative
        //    *                       to list of ParentListItems only.
        //    */
        //public void notifyParentItemRemoved(int parentPosition)
        //{
        //    int wrapperIndex = getParentWrapperIndex(parentPosition);
        //    int sizeChanged = removeParentWrapper(wrapperIndex);

        //    notifyItemRangeRemoved(wrapperIndex, sizeChanged);
        //}

        ///**
        //    * Notify any registered observers that the {@code itemCount} ParentListItems previously located
        //    * at {@code parentPositionStart} have been removed from the data set. The ParentListItems
        //    * previously located at and after {@code parentPositionStart + itemCount} may now be found at
        //    * {@code oldPosition - itemCount}.
        //    * <p>
        //    * This is a structural change event. Representations of other existing items in the
        //    * data set are still considered up to date and will not be rebound, though their positions
        //    * may be altered.
        //    *
        //    * @param parentPositionStart The previous position of the first ParentListItem that was
        //    *                            removed, relative to list of ParentListItems only.
        //    * @param itemCount Number of ParentListItems removed from the data set
        //    */
        //public void notifyParentItemRangeRemoved(int parentPositionStart, int itemCount)
        //{
        //    int sizeChanged = 0;
        //    int wrapperIndex = getParentWrapperIndex(parentPositionStart);
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        sizeChanged += removeParentWrapper(wrapperIndex);
        //    }

        //    notifyItemRangeRemoved(wrapperIndex, sizeChanged);
        //}

        //private int removeParentWrapper(int parentWrapperIndex)
        //{
        //    int sizeChanged = 1;
        //    ParentWrapper parentWrapper = (ParentWrapper)mItemList.remove(parentWrapperIndex);
        //    if (parentWrapper.isExpanded())
        //    {
        //        int childListSize = parentWrapper.getChildItemList().size();
        //        for (int i = 0; i < childListSize; i++)
        //        {
        //            mItemList.remove(parentWrapperIndex);
        //            sizeChanged++;
        //        }
        //    }
        //    return sizeChanged;
        //}

        ///**
        //    * Notify any registered observers that the ParentListItem at {@code parentPosition} has changed.
        //    * This will also trigger an item changed for children of the ParentList specified.
        //    * <p>
        //    * This is an item change event, not a structural change event. It indicates that any
        //    * reflection of the data at {@code parentPosition} is out of date and should be updated.
        //    * The ParentListItem at {@code parentPosition} retains the same identity. This means
        //    * the number of children must stay the same.
        //    *
        //    * @param parentPosition Position of the item that has changed
        //    */
        //public void notifyParentItemChanged(int parentPosition)
        //{
        //    ParentListItem parentListItem = mParentItemList.get(parentPosition);
        //    int wrapperIndex = getParentWrapperIndex(parentPosition);
        //    int sizeChanged = changeParentWrapper(wrapperIndex, parentListItem);

        //    notifyItemRangeChanged(wrapperIndex, sizeChanged);
        //}

        ///**
        //    * Notify any registered observers that the {@code itemCount} ParentListItems starting
        //    * at {@code parentPositionStart} have changed. This will also trigger an item changed
        //    * for children of the ParentList specified.
        //    * <p>
        //    * This is an item change event, not a structural change event. It indicates that any
        //    * reflection of the data in the given position range is out of date and should be updated.
        //    * The ParentListItems in the given range retain the same identity. This means
        //    * the number of children must stay the same.
        //    *
        //    * @param parentPositionStart Position of the item that has changed
        //    * @param itemCount Number of ParentListItems changed in the dataset
        //    */
        //public void notifyParentItemRangeChanged(int parentPositionStart, int itemCount)
        //{
        //    int initialWrapperIndex = getParentWrapperIndex(parentPositionStart);

        //    int wrapperIndex = initialWrapperIndex;
        //    int sizeChanged = 0;
        //    int changed;
        //    ParentListItem parentListItem;
        //    for (int j = 0; j < itemCount; j++)
        //    {
        //        parentListItem = mParentItemList.get(parentPositionStart);
        //        changed = changeParentWrapper(wrapperIndex, parentListItem);
        //        sizeChanged += changed;
        //        wrapperIndex += changed;
        //        parentPositionStart++;
        //    }
        //    notifyItemRangeChanged(initialWrapperIndex, sizeChanged);
        //}

        //private int changeParentWrapper(int wrapperIndex, ParentListItem parentListItem)
        //{
        //    ParentWrapper parentWrapper = (ParentWrapper)mItemList.get(wrapperIndex);
        //    parentWrapper.setParentListItem(parentListItem);
        //    int sizeChanged = 1;
        //    if (parentWrapper.isExpanded())
        //    {
        //        List <?> childItems = parentWrapper.getChildItemList();
        //        int childListSize = childItems.size();
        //        Object child;
        //        for (int i = 0; i < childListSize; i++)
        //        {
        //            child = childItems.get(i);
        //            mItemList.set(wrapperIndex + i + 1, child);
        //            sizeChanged++;
        //        }
        //    }

        //    return sizeChanged;

        //}

        ///**
        //    * Notify any registered observers that the ParentListItem and it's child list items reflected at
        //    * {@code fromParentPosition} has been moved to {@code toParentPosition}.
        //    *
        //    * <p>This is a structural change event. Representations of other existing items in the
        //    * data set are still considered up to date and will not be rebound, though their
        //    * positions may be altered.</p>
        //    *
        //    * @param fromParentPosition Previous position of the ParentListItem, relative to list of
        //    *                           ParentListItems only.
        //    * @param toParentPosition New position of the ParentListItem, relative to list of
        //    *                         ParentListItems only.
        //    */
        //public void notifyParentItemMoved(int fromParentPosition, int toParentPosition)
        //{

        //    int fromWrapperIndex = getParentWrapperIndex(fromParentPosition);
        //    ParentWrapper fromParentWrapper = (ParentWrapper)mItemList.get(fromWrapperIndex);

        //    // If the parent is collapsed we can take advantage of notifyItemMoved otherwise
        //    // we are forced to do a "manual" move by removing and then adding the parent + children
        //    // (no notifyItemRangeMovedAvailable)
        //    boolean isCollapsed = !fromParentWrapper.isExpanded();
        //    boolean isExpandedNoChildren = !isCollapsed && (fromParentWrapper.getChildItemList().size() == 0);
        //    if (isCollapsed || isExpandedNoChildren)
        //    {
        //        int toWrapperIndex = getParentWrapperIndex(toParentPosition);
        //        ParentWrapper toParentWrapper = (ParentWrapper)mItemList.get(toWrapperIndex);
        //        mItemList.remove(fromWrapperIndex);
        //        int childOffset = 0;
        //        if (toParentWrapper.isExpanded())
        //        {
        //            childOffset = toParentWrapper.getChildItemList().size();
        //        }
        //        mItemList.add(toWrapperIndex + childOffset, fromParentWrapper);

        //        notifyItemMoved(fromWrapperIndex, toWrapperIndex + childOffset);
        //    }
        //    else {
        //        // Remove the parent and children
        //        int sizeChanged = 0;
        //        int childListSize = fromParentWrapper.getChildItemList().size();
        //        for (int i = 0; i < childListSize + 1; i++)
        //        {
        //            mItemList.remove(fromWrapperIndex);
        //            sizeChanged++;
        //        }
        //        notifyItemRangeRemoved(fromWrapperIndex, sizeChanged);


        //        // Add the parent and children at new position
        //        int toWrapperIndex = getParentWrapperIndex(toParentPosition);
        //        int childOffset = 0;
        //        if (toWrapperIndex != -1)
        //        {
        //            ParentWrapper toParentWrapper = (ParentWrapper)mItemList.get(toWrapperIndex);
        //            if (toParentWrapper.isExpanded())
        //            {
        //                childOffset = toParentWrapper.getChildItemList().size();
        //            }
        //        }
        //        else {
        //            toWrapperIndex = mItemList.size();
        //        }
        //        mItemList.add(toWrapperIndex + childOffset, fromParentWrapper);
        //        List <?> childItemList = fromParentWrapper.getChildItemList();
        //        sizeChanged = childItemList.size() + 1;
        //        mItemList.addAll(toWrapperIndex + childOffset + 1, childItemList);
        //        notifyItemRangeInserted(toWrapperIndex + childOffset, sizeChanged);
        //    }
        //}

        ///**
        //    * Notify any registered observers that the ParentListItem reflected at {@code parentPosition}
        //    * has a child list item that has been newly inserted at {@code childPosition}.
        //    * The child list item previously at {@code childPosition} is now at
        //    * position {@code childPosition + 1}.
        //    * <p>
        //    * This is a structural change event. Representations of other existing items in the
        //    * data set are still considered up to date and will not be rebound, though their
        //    * positions may be altered.
        //    *
        //    * @param parentPosition Position of the ParentListItem which has been added a child, relative
        //    *                       to list of ParentListItems only.
        //    * @param childPosition Position of the child object that has been inserted, relative to children
        //    *                      of the ParentListItem specified by {@code parentPosition} only.
        //    *
        //    */
        //public void notifyChildItemInserted(int parentPosition, int childPosition)
        //{
        //    int parentWrapperIndex = getParentWrapperIndex(parentPosition);
        //    ParentWrapper parentWrapper = (ParentWrapper)mItemList.get(parentWrapperIndex);

        //    if (parentWrapper.isExpanded())
        //    {
        //        ParentListItem parentListItem = mParentItemList.get(parentPosition);
        //        Object child = parentListItem.getChildItemList().get(childPosition);
        //        mItemList.add(parentWrapperIndex + childPosition + 1, child);
        //        notifyItemInserted(parentWrapperIndex + childPosition + 1);
        //    }
        //}

        ///**
        //    * Notify any registered observers that the ParentListItem reflected at {@code parentPosition}
        //    * has {@code itemCount} child list items that have been newly inserted at {@code childPositionStart}.
        //    * The child list item previously at {@code childPositionStart} and beyond are now at
        //    * position {@code childPositionStart + itemCount}.
        //    * <p>
        //    * This is a structural change event. Representations of other existing items in the
        //    * data set are still considered up to date and will not be rebound, though their
        //    * positions may be altered.
        //    *
        //    * @param parentPosition Position of the ParentListItem which has been added a child, relative
        //    *                       to list of ParentListItems only.
        //    * @param childPositionStart Position of the first child object that has been inserted,
        //    *                           relative to children of the ParentListItem specified by
        //    *                           {@code parentPosition} only.
        //    * @param itemCount number of children inserted
        //    *
        //    */
        //public void notifyChildItemRangeInserted(int parentPosition, int childPositionStart, int itemCount)
        //{
        //    int parentWrapperIndex = getParentWrapperIndex(parentPosition);
        //    ParentWrapper parentWrapper = (ParentWrapper)mItemList.get(parentWrapperIndex);

        //    if (parentWrapper.isExpanded())
        //    {
        //        ParentListItem parentListItem = mParentItemList.get(parentPosition);
        //        List <?> childList = parentListItem.getChildItemList();
        //        Object child;
        //        for (int i = 0; i < itemCount; i++)
        //        {
        //            child = childList.get(childPositionStart + i);
        //            mItemList.add(parentWrapperIndex + childPositionStart + i + 1, child);
        //        }
        //        NotifyItemRangeInserted(parentWrapperIndex + childPositionStart + 1, itemCount);
        //    }
        //}

        ///**
        //    * Notify any registered observers that the ParentListItem located at {@code parentPosition}
        //    * has a child list item that has been removed from the data set, previously located at {@code childPosition}.
        //    * The child list item previously located at and after {@code childPosition} may
        //    * now be found at {@code childPosition - 1}.
        //    * <p>
        //    * This is a structural change event. Representations of other existing items in the
        //    * data set are still considered up to date and will not be rebound, though their positions
        //    * may be altered.
        //    *
        //    * @param parentPosition Position of the ParentListItem which has a child removed from, relative
        //    *                       to list of ParentListItems only.
        //    * @param childPosition Position of the child object that has been removed, relative to children
        //    *                      of the ParentListItem specified by {@code parentPosition} only.
        //    */
        //public void notifyChildItemRemoved(int parentPosition, int childPosition)
        //{
        //    int parentWrapperIndex = getParentWrapperIndex(parentPosition);
        //    ParentWrapper parentWrapper = (ParentWrapper)mItemList.get(parentWrapperIndex);

        //    if (parentWrapper.isExpanded())
        //    {
        //        mItemList.remove(parentWrapperIndex + childPosition + 1);
        //        notifyItemRemoved(parentWrapperIndex + childPosition + 1);
        //    }
        //}

        ///**
        //    * Notify any registered observers that the ParentListItem located at {@code parentPosition}
        //    * has {@code itemCount} child list items that have been removed from the data set, previously
        //    * located at {@code childPositionStart} onwards. The child list item previously located at and
        //    * after {@code childPositionStart} may now be found at {@code childPositionStart - itemCount}.
        //    * <p>
        //    * This is a structural change event. Representations of other existing items in the
        //    * data set are still considered up to date and will not be rebound, though their positions
        //    * may be altered.
        //    *
        //    * @param parentPosition Position of the ParentListItem which has a child removed from, relative
        //    *                       to list of ParentListItems only.
        //    * @param childPositionStart Position of the first child object that has been removed, relative
        //    *                           to children of the ParentListItem specified by
        //    *                           {@code parentPosition} only.
        //    * @param itemCount number of children removed
        //    */
        //public void notifyChildItemRangeRemoved(int parentPosition, int childPositionStart, int itemCount)
        //{
        //    int parentWrapperIndex = getParentWrapperIndex(parentPosition);
        //    ParentWrapper parentWrapper = (ParentWrapper)mItemList.get(parentWrapperIndex);

        //    if (parentWrapper.isExpanded())
        //    {
        //        for (int i = 0; i < itemCount; i++)
        //        {
        //            mItemList.remove(parentWrapperIndex + childPositionStart + 1);
        //        }
        //        notifyItemRangeRemoved(parentWrapperIndex + childPositionStart + 1, itemCount);
        //    }
        //}

        ///**
        //    * Notify any registered observers that the ParentListItem at {@code parentPosition} has
        //    * a child located at {@code childPosition} that has changed.
        //    * <p>
        //    * This is an item change event, not a structural change event. It indicates that any
        //    * reflection of the data at {@code childPosition} is out of date and should be updated.
        //    * The ParentListItem at {@code childPosition} retains the same identity.
        //    *
        //    * @param parentPosition Position of the ParentListItem who has a child that has changed
        //    * @param childPosition Position of the child that has changed
        //    */
        //public void notifyChildItemChanged(int parentPosition, int childPosition)
        //{
        //    ParentListItem parentListItem = mParentItemList.get(parentPosition);
        //    int parentWrapperIndex = getParentWrapperIndex(parentPosition);
        //    ParentWrapper parentWrapper = (ParentWrapper)mItemList.get(parentWrapperIndex);
        //    parentWrapper.setParentListItem(parentListItem);
        //    if (parentWrapper.isExpanded())
        //    {
        //        int listChildPosition = parentWrapperIndex + childPosition + 1;
        //        Object child = parentWrapper.getChildItemList().get(childPosition);
        //        mItemList.set(listChildPosition, child);
        //        notifyItemChanged(listChildPosition);
        //    }
        //}

        ///**
        //    * Notify any registered observers that the ParentListItem at {@code parentPosition} has
        //    * {@code itemCount} child Objects starting at {@code childPositionStart} that have changed.
        //    * <p>
        //    * This is an item change event, not a structural change event. It indicates that any
        //    * The ParentListItem at {@code childPositionStart} retains the same identity.
        //    * reflection of the set of {@code itemCount} child objects starting at {@code childPositionStart}
        //    * are out of date and should be updated.
        //    *
        //    * @param parentPosition Position of the ParentListItem who has a child that has changed
        //    * @param childPositionStart Position of the first child object that has changed
        //    * @param itemCount number of child objects changed
        //    */
        //public void notifyChildItemRangeChanged(int parentPosition, int childPositionStart, int itemCount)
        //{
        //    ParentListItem parentListItem = mParentItemList.get(parentPosition);
        //    int parentWrapperIndex = getParentWrapperIndex(parentPosition);
        //    ParentWrapper parentWrapper = (ParentWrapper)mItemList.get(parentWrapperIndex);
        //    parentWrapper.setParentListItem(parentListItem);
        //    if (parentWrapper.isExpanded())
        //    {
        //        int listChildPosition = parentWrapperIndex + childPositionStart + 1;
        //        for (int i = 0; i < itemCount; i++)
        //        {
        //            Object child = parentWrapper.getChildItemList().get(childPositionStart + i);
        //            mItemList.set(listChildPosition + i, child);

        //        }
        //        notifyItemRangeChanged(listChildPosition, itemCount);
        //    }
        //}

        ///**
        //    * Notify any registered observers that the child list item contained within the ParentListItem
        //    * at {@code parentPosition} has moved from {@code fromChildPosition} to {@code toChildPosition}.
        //    *
        //    * <p>This is a structural change event. Representations of other existing items in the
        //    * data set are still considered up to date and will not be rebound, though their
        //    * positions may be altered.</p>
        //    *
        //    * @param parentPosition Position of the ParentListItem who has a child that has moved
        //    * @param fromChildPosition Previous position of the child list item
        //    * @param toChildPosition New position of the child list item
        //    */
        //public void notifyChildItemMoved(int parentPosition, int fromChildPosition, int toChildPosition)
        //{
        //    ParentListItem parentListItem = mParentItemList.get(parentPosition);
        //    int parentWrapperIndex = getParentWrapperIndex(parentPosition);
        //    ParentWrapper parentWrapper = (ParentWrapper)mItemList.get(parentWrapperIndex);
        //    parentWrapper.setParentListItem(parentListItem);
        //    if (parentWrapper.isExpanded())
        //    {
        //        Object fromChild = mItemList.remove(parentWrapperIndex + 1 + fromChildPosition);
        //        mItemList.add(parentWrapperIndex + 1 + toChildPosition, fromChild);
        //        notifyItemMoved(parentWrapperIndex + 1 + fromChildPosition, parentWrapperIndex + 1 + toChildPosition);
        //    }
        //}

        #endregion
    }
}