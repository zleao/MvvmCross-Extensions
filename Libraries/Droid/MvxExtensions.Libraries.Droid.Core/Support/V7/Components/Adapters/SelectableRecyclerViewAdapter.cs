using Android.Support.V7.Widget;
using Android.Views;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.SelectableRecyclerViewComponents;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Binding.ExtensionMethods;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform.WeakSubscription;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Adapters
{
    public class SelectableRecyclerViewAdapter : MvxRecyclerAdapter
    {
        #region Fields

        private MvxNotifyCollectionChangedEventSubscription _selectedItemscollectionChangedToken = null;

        protected readonly SelectableViewHolderManager _selectableViewHolderManager = new SelectableViewHolderManager();

        #endregion

        #region Properties

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

        #endregion

        #region Commands

        protected virtual ICommand MyItemClick
        {
            get { return _myItemClick ?? (_myItemClick = new MvxCommand<object>(OnItemClick)); }
        }
        private ICommand _myItemClick;

        #endregion

        #region Constructor

        public SelectableRecyclerViewAdapter()
            : this(MvxAndroidBindingContextHelpers.Current())
        {
        }

        public SelectableRecyclerViewAdapter(IMvxAndroidBindingContext bindingContext)
            : base(bindingContext)
        {
        }

        #endregion

        #region Methods

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
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to SelectableRecyclerViewAdapter.SelectedItems binding");
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
                    var pos = GetPosition(item);
                    if (pos >= 0)
                        SetItemSelection(pos, true);
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
                            var itemPosition = GetPosition(item);
                            if (itemPosition >= 0)
                                SetItemSelection(itemPosition, true);
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                        Android.Util.Log.Error("SelectableRecyclerViewAdapter.OnSelectedItemsCollectionChanged", "NotifyCollectionChangedAction.Move not supported");
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in e.OldItems)
                        {
                            var itemPosition = GetPosition(item);
                            if (itemPosition >= 0)
                                SetItemSelection(itemPosition, false);
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        Android.Util.Log.Error("SelectableRecyclerViewAdapter.OnSelectedItemsCollectionChanged", "NotifyCollectionChangedAction.Replace not supported");
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        for (int i = 0; i < ItemCount; i++)
                        {
                            SetItemSelection(i, false);
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Android.Util.Log.Wtf("SelectableRecyclerViewAdapter.OnSelectedItemsCollectionChanged", ex.Message);
            }
        }

        protected override void SetItemsSource(IEnumerable value)
        {
            _selectableViewHolderManager.SetItemsSource(value, SelectedItems);

            base.SetItemsSource(value);
        }
        protected override void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            _selectableViewHolderManager.OnItemsSourceCollectionChanged(args);

            base.OnItemsSourceCollectionChanged(sender, args);

            try
            {
                if (args != null)
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            //No need for extra work here. The Source list should update itself and there's no impact in the selected items
                            break;

                        case NotifyCollectionChangedAction.Move:
                            Android.Util.Log.Error("SelectableRecyclerViewAdapter.OnItemsSourceChanged", "NotifyCollectionChangedAction.Move not supported");
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            if (SelectedItems != null)
                            {
                                foreach (var item in args.OldItems)
                                {
                                    if (SelectedItems.Contains(item))
                                        SelectedItems.Remove(item);
                                }
                            }
                            break;

                        case NotifyCollectionChangedAction.Replace:
                            Android.Util.Log.Error("SelectableRecyclerViewAdapter.OnItemsSourceChanged", "NotifyCollectionChangedAction.Replace not supported");
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            SelectedItems.SafeClear();
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Android.Util.Log.Wtf("SelectableRecyclerView.OnItemsSourceChanged", ex.Message);
            }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext.LayoutInflaterHolder);

            var newHolder = _selectableViewHolderManager.CreateNewHolder(InflateViewForHolder(parent, viewType, itemBindingContext), itemBindingContext);

            newHolder.Click = MyItemClick;
            newHolder.LongClick = this.ItemLongClick;

            return newHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            base.OnBindViewHolder(holder, position);   

            _selectableViewHolderManager.BindHolder(holder as SelectableViewHolder, position, SingleSelection, SelectionEnabled);
        }


        public virtual int GetPosition(object item)
        {
            return ItemsSource.GetPosition(item);
        }


        protected virtual void SetItemSelection(int itemPosition, bool value)
        {
            _selectableViewHolderManager.SetItemSelection(itemPosition, value);
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
                var itemPosition = GetPosition(clickedItem);

                if (SelectionEnabled)
                {
                    if (SelectedItems == null)
                        return;

                    var isSelected = _selectableViewHolderManager.IsItemSelected(itemPosition);
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

        public override void OnViewDetachedFromWindow(Java.Lang.Object holder)
        {
            base.OnViewDetachedFromWindow(holder);
        }

        #endregion
    }
}