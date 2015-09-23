using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Cirrious.CrossCore.WeakSubscription;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Cirrious.MvvmCross.Binding;
using Cirrious.CrossCore.Platform;
using System.Collections.Specialized;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters;
using System.Windows.Input;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views
{
    public class SelectableListView : MvxListView
    {
        #region Fields

        private MvxNotifyCollectionChangedEventSubscription _collectionChangedToken = null;
        private bool _ignoreSelectedItemsChanged = false;

        #endregion

        #region Properties

        public bool SingleSelection
        {
            get { return _singleSelection; }
            set { SetSingleSelection(value); }
        }
        private bool _singleSelection;

        public IList SelectedItems
        {
            get { return _selectedItems; }
            set { SetSelectedItems(value); }
        }
        private IList _selectedItems;

        protected SelectableListViewAdapter TypedAdapter
        {
            get { return Adapter as SelectableListViewAdapter; }
        }

        #endregion

        #region Commands

        public ICommand SelectItemCommand { get; set; }

        #endregion

        #region Constructors

        public SelectableListView(Context context, IAttributeSet attrs)
            : this(context, attrs, new SelectableListViewAdapter(context))
        {
        }

        public SelectableListView(Context context, IAttributeSet attrs, SelectableListViewAdapter adapter)
            : base(context, attrs, adapter)
        {
            ((ListView)this).ItemClick += OnItemClicked;
            this.ChoiceMode = _singleSelection ? ChoiceMode.Single : ChoiceMode.Multiple;
            TypedAdapter.OnItemsSourceChanged = OnItemsSourceChanged;
        }

        #endregion

        #region Methods

        protected void OnItemsSourceChanged(NotifyCollectionChangedEventArgs args)
        {
            try
            {
                if (args != null)
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            Android.Util.Log.Error("SelectableListView.OnItemsSourceChanged", "NotifyCollectionChangedAction.Add not supported");
                            break;

                        case NotifyCollectionChangedAction.Move:
                            Android.Util.Log.Error("SelectableListView.OnItemsSourceChanged", "NotifyCollectionChangedAction.Move not supported");
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            if (SelectedItems != null)
                            {
                                _ignoreSelectedItemsChanged = true;
                                foreach (var item in args.OldItems)
                                {
                                    if (SelectedItems.Contains(item))
                                        SelectedItems.Remove(item);
                                }
                            }
                            UpdateItemsVisualState();
                            break;

                        case NotifyCollectionChangedAction.Replace:
                            Android.Util.Log.Error("SelectableListView.OnItemsSourceChanged", "NotifyCollectionChangedAction.Replace not supported");
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            Android.Util.Log.Error("SelectableListView.OnItemsSourceChanged", "NotifyCollectionChangedAction.Reset not supported");
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Wtf("SelectableListView.OnItemsSourceChanged", ex.Message);
            }
            finally
            {
                _ignoreSelectedItemsChanged = false;
            }
        }

        private void UpdateItemsVisualState()
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

        protected virtual void SetSingleSelection(bool value)
        {
            if (_singleSelection != value)
            {
                _singleSelection = value;

                this.ChoiceMode = _singleSelection ? ChoiceMode.Single : ChoiceMode.Multiple;

                if(_singleSelection && (SelectedItems == null || SelectedItems.Count > 1))
                    SetAllItemsChecked(false);

                InvalidateViews();
            }
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
                Android.Util.Log.Wtf("SelectableListView.OnItemClicked", ex.Message);
            }
            finally
            {
                _ignoreSelectedItemsChanged = false;
            }
        }

        public IEnumerable GetSelectedItems()
        {
            return SelectedItems;
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
                            Android.Util.Log.Error("SelectableListView.OnSelectedItemsCollectionChanged", "NotifyCollectionChangedAction.Move not supported");
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
                            Android.Util.Log.Error("SelectableListView.OnSelectedItemsCollectionChanged", "NotifyCollectionChangedAction.Replace not supported");
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
                Android.Util.Log.Wtf("SelectableListView.OnSelectedItemsCollectionChanged", ex.Message);
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

        protected override bool DrawChild(Canvas canvas, Android.Views.View child, long drawingTime)
        {
            var liChild = child as MvxListItemView;
            if (liChild != null)
            {
                var singleImage = child.FindViewWithTag("SingleSelectionImage") as ImageView;
                var multiImage = child.FindViewWithTag("MultiSelectionImage") as ImageView;

                if (singleImage != null)
                    singleImage.Visibility = (SingleSelection ? ViewStates.Visible : ViewStates.Gone);

                if (multiImage != null)
                    multiImage.Visibility = (SingleSelection ? ViewStates.Gone : ViewStates.Visible);
            }

            return base.DrawChild(canvas, child, drawingTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((ListView)this).ItemClick -= OnItemClicked;
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