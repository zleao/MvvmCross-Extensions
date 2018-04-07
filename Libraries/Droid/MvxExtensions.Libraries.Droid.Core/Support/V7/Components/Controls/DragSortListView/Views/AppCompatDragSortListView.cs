using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Adapters;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView.Interfaces;
using MvvmCross.Binding;
using MvvmCross.Binding.Attributes;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform.WeakSubscription;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView
{
    public class AppCompatDragSortListView : AppCompatBaseDragSortListView
    {
        #region Fields

        private MvxNotifyCollectionChangedEventSubscription _collectionChangedToken = null;
        private bool _ignoreSelectedItemsChanged = false;

        #endregion

        #region Properties

        public new AppCompatDragSortListViewAdapter Adapter
        {
            get { return base.Adapter as AppCompatDragSortListViewAdapter; }
            set
            {
                var existing = Adapter;
                if (existing == value)
                    return;

                if (!(value is AppCompatDragSortListViewAdapter))
                {
                    throw new NotSupportedException("The adapter is not of AppCompatDragSortListViewAdapter type");
                }

                base.Adapter = value;

                if (Adapter != null)
                {
                    if (typeof(IDropListener).IsAssignableFrom(Adapter.GetType()))
                        DropListener = Adapter as IDropListener;

                    if (typeof(IDragListener).IsAssignableFrom(Adapter.GetType()))
                        DragListener = Adapter as IDragListener;

                    if (typeof(IRemoveListener).IsAssignableFrom(Adapter.GetType()))
                        RemoveListener = Adapter as IRemoveListener;
                }
                else
                {
                    DropListener = null;
                    DragListener = null;
                    RemoveListener = null;
                }
            }
        }

        [MvxSetToNullAfterBinding]
        public IEnumerable ItemsSource
        {
            get { return Adapter.ItemsSource; }
            set { Adapter.ItemsSource = value; }
        }

        public IList SelectedItems
        {
            get { return _selectedItems; }
            set { SetSelectedItems(value); }
        }
        private IList _selectedItems;

        public bool SelectionEnabled
        {
            get { return _selectionEnabled; }
            set { SetSelectionEnabled(value); }
        }
        private bool _selectionEnabled;

        public bool SingleSelection
        {
            get { return _singleSelection; }
            set { SetSingleSelection(value); }
        }
        private bool _singleSelection;

        #endregion

        #region Commands

        public ICommand SelectItemCommand { get; set; }

        #endregion

        #region Constructor

        public AppCompatDragSortListView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            var itemTemplateId = MvxAttributeHelpers.ReadListItemTemplateId(context, attrs);
            Adapter = new AppCompatDragSortListViewAdapter(context, this, itemTemplateId);

            (this).ItemClick += OnItemClicked;
        }

        #endregion

        #region Methods

        protected override bool DrawChild(Canvas canvas, View child, long drawingTime)
        {
            var liChild = child as DragSortItemView;
            if (liChild != null)
            {
                var singleImage = child.FindViewWithTag("SingleSelectionImage") as ImageView;
                var multiImage = child.FindViewWithTag("MultiSelectionImage") as ImageView;

                if (singleImage != null)
                    singleImage.Visibility = SelectionEnabled ? (SingleSelection ? ViewStates.Visible : ViewStates.Gone) : ViewStates.Gone;

                if (multiImage != null)
                    multiImage.Visibility = SelectionEnabled ? (SingleSelection ? ViewStates.Gone : ViewStates.Visible) : ViewStates.Gone;
            }

            return base.DrawChild(canvas, child, drawingTime);
        }

        public void UpdateItemsVisualState()
        {
            if (SelectedItems != null)
            {
                var isSelectedItemsNullOrEmpty = (SelectedItems == null || SelectedItems.Count == 0);
                foreach (var item in Adapter.ItemsSource)
                {
                    var pos = Adapter.GetPosition(item);
                    SetItemChecked(pos, (isSelectedItemsNullOrEmpty ? false : SelectedItems.Contains(item)));
                }
            }
        }

        protected virtual void SetSelectionEnabled(bool value)
        {
            if (_selectionEnabled != value)
            {
                _selectionEnabled = value;
                this.ChoiceMode = _selectionEnabled ? (_singleSelection ? ChoiceMode.Single : ChoiceMode.Multiple) : ChoiceMode.None;
                SetAllItemsChecked(false);
                InvalidateViews();
            }
        }

        protected virtual void SetSingleSelection(bool value)
        {
            if (_singleSelection != value)
            {
                _singleSelection = value;

                this.ChoiceMode = _singleSelection ? ChoiceMode.Single : ChoiceMode.Multiple;

                if (SelectionEnabled)
                {
                    if (_singleSelection && (SelectedItems == null || SelectedItems.Count > 1))
                        SetAllItemsChecked(false);

                    InvalidateViews();
                }
            }
        }

        protected void SetSelectedItems(IList items)
        {
            if (_collectionChangedToken != null)
            {
                _collectionChangedToken.Dispose();
                _collectionChangedToken = null;
            }

            if (items == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to AppCompatDragSortListView.SelectedItems binding");
                return;
            }

            if (typeof(INotifyCollectionChanged).IsAssignableFrom(items.GetType()))
            {
                _collectionChangedToken = (items as INotifyCollectionChanged).WeakSubscribe(OnSelectedItemsCollectionChanged);
            }

            _selectedItems = items;
            if (_selectedItems.Count > 0)
            {
                foreach (var item in _selectedItems)
                {
                    var itemPosition = Adapter.GetPosition(item);
                    if (itemPosition >= 0)
                        this.SetItemChecked(itemPosition, true);
                }
            }
            else
            {
                SetAllItemsChecked(false);
            }
        }

        private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (!_ignoreSelectedItemsChanged)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var item in e.NewItems)
                            {
                                var itemPosition = Adapter.GetPosition(item);
                                if (itemPosition >= 0)
                                    this.SetItemChecked(itemPosition, true);
                            }
                            break;

                        case NotifyCollectionChangedAction.Move:
                            Android.Util.Log.Error("AppCompatDragSortListView.OnSelectedItemsCollectionChanged", "NotifyCollectionChangedAction.Move not supported");
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            foreach (var item in e.OldItems)
                            {
                                var itemPosition = Adapter.GetPosition(item);
                                if (itemPosition >= 0)
                                    this.SetItemChecked(itemPosition, false);
                            }
                            break;

                        case NotifyCollectionChangedAction.Replace:
                            Android.Util.Log.Error("AppCompatDragSortListView.OnSelectedItemsCollectionChanged", "NotifyCollectionChangedAction.Replace not supported");
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            for (int i = 0; i < Adapter.Count; i++)
                            {
                                this.SetItemChecked(i, false);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Wtf("AppCompatDragSortListView.OnSelectedItemsCollectionChanged", ex.Message);
            }
        }

        private void SetAllItemsChecked(bool value)
        {
            for (int i = 0; i < this.ChildCount; i++)
            {
                this.SetItemChecked(i, value);

                if (value)
                {
                    var objItem = this.GetItemAtPosition(i);

                    if (SelectedItems != null && !SelectedItems.Contains(objItem))
                        SelectedItems.Add(objItem);
                }
            }

            if (!value && SelectedItems != null && SelectedItems.Count > 0)
                SelectedItems.Clear();
        }

        private void OnItemClicked(object sender, ItemClickEventArgs e)
        {
            var liv = e.View as MvxListItemView;
            if (liv == null || SelectedItems == null)
                return;

            _ignoreSelectedItemsChanged = true;

            //get clicked item
            var item = Adapter.GetRawItem(e.Position);

            try
            {
                if (!liv.Checked)
                {
                    //if the item was unselected, remove it from the selected items list
                    if (SelectedItems.Contains(item))
                    {
                        SelectedItems.Remove(item);
                        if (SelectItemCommand != null && SelectItemCommand.CanExecute(item))
                            SelectItemCommand.Execute(item);
                    }
                }
                else
                {
                    var canSelectItem = true;
                    if (SelectItemCommand != null)
                        canSelectItem = SelectItemCommand.CanExecute(item);

                    if (canSelectItem)
                    {
                        if (!SelectedItems.Contains(item))
                        {
                            if (SingleSelection)
                                SelectedItems.Clear();
                            SelectedItems.Add(item);

                            if (SelectItemCommand != null)
                                SelectItemCommand.Execute(item);
                        }
                    }
                    else
                    {
                        //If the item can't be added to the selected items,
                        //we must set the correspondent view to 'uncheked' state
                        this.SetItemChecked(e.Position, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Wtf("AppCompatDragSortListView.OnItemClicked", ex.Message);
            }
            finally
            {
                _ignoreSelectedItemsChanged = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_collectionChangedToken != null)
                {
                    _collectionChangedToken.Dispose();
                    _collectionChangedToken = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}