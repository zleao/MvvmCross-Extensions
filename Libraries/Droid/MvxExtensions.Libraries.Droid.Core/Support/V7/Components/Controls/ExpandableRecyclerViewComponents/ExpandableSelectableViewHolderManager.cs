using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.SelectableRecyclerViewComponents;
using MvxExtensions.Libraries.Portable.Core.Models;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.ExpandableRecyclerViewComponents
{
    public class ExpandableSelectableViewHolderManager : SelectableViewHolderManager
    {
        public override void SetItemsSource(IEnumerable itemsToWrap, IList selectedItems = null)
        {
            _selectableItems.Clear();
            if (itemsToWrap.SafeCount() > 0)
            {
                foreach (IGroupItem groupItem in itemsToWrap)
                {
                    if (groupItem == null)
                        throw new InvalidCastException("itemsToWrap must be a list of IGroupItem");

                    if (groupItem.Children.SafeCount() > 0)
                    {
                        foreach (var itemChild in groupItem.Children)
                        {
                            var selectableItem = new SelectableItem(itemChild);

                            if (selectedItems != null)
                                selectableItem.IsSelected = selectedItems.Contains(itemChild);

                            _selectableItems.Add(selectableItem);
                        }
                    }
                }
            }
            ExecuteOnAllTrackedHolders(h => h.Reset());
        }

        public override void OnItemsSourceCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            try
            {
                if (args != null)
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (IGroupItem groupItem in args.NewItems)
                            {
                                if (groupItem == null)
                                    throw new InvalidCastException("Added Items must be of type IGroupItem");

                                if (groupItem.Children.SafeCount() > 0)
                                {
                                    foreach (var itemChild in groupItem.Children)
                                    {
                                        _selectableItems.Add(new SelectableItem(itemChild));
                                    }
                                }
                            }
                            break;

                        case NotifyCollectionChangedAction.Move:
                            Android.Util.Log.Error("ExpandableSelectableViewHolderManager.OnItemsSourceChanged", "NotifyCollectionChangedAction.Move not supported");
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            foreach (IGroupItem groupItem in args.OldItems)
                            {
                                if (groupItem == null)
                                    throw new InvalidCastException("Added Items must be of type IGroupItem");

                                if (groupItem.Children.SafeCount() > 0)
                                {
                                    foreach (var itemChild in groupItem.Children)
                                    {
                                        var selItem = _selectableItems.SafeFirstOrDefault(i => i.Item != null ? i.Item.Equals(itemChild) : false);
                                        if (selItem != null)
                                            _selectableItems.RemoveAll(i => i.Item != null ? i.Item.Equals(itemChild) : false);
                                    }
                                }
                            }
                            break;

                        case NotifyCollectionChangedAction.Replace:
                            Android.Util.Log.Error("ExpandableSelectableViewHolderManager.OnItemsSourceChanged", "NotifyCollectionChangedAction.Replace not supported");
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            SetItemsSource(null);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Wtf("SelectableRecyclerView.OnItemsSourceChanged", ex.Message);
            }
        }

        public virtual void OnItemsSourceChildCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            try
            {
                if (args != null)
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Move:
                            Android.Util.Log.Error("ExpandableSelectableViewHolderManager.OnItemsSourceChildChanged", "NotifyCollectionChangedAction.Move not supported");
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            Android.Util.Log.Error("ExpandableSelectableViewHolderManager.OnItemsSourceChildChanged", "NotifyCollectionChangedAction.Replace not supported");
                            break;

                        case NotifyCollectionChangedAction.Add:
                            foreach (var itemChild in args.NewItems)
                            {
                                _selectableItems.Add(new SelectableItem(itemChild));
                            }
                            break;

                        case NotifyCollectionChangedAction.Remove:
                        case NotifyCollectionChangedAction.Reset:
                            foreach (var itemChild in args.OldItems)
                            {
                                var selItem = _selectableItems.SafeFirstOrDefault(i => i.Item != null ? i.Item.Equals(itemChild) : false);
                                if (selItem != null)
                                    _selectableItems.RemoveAll(i => i.Item != null ? i.Item.Equals(itemChild) : false);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Wtf("SelectableRecyclerView.OnItemsSourceChildChanged", ex.Message);
            }
        }

        public void SetItemSelection(object item, bool value)
        {
            var selItem = _selectableItems.SafeFirstOrDefault(si => si.Item != null && si.Item.Equals(item));
            if (selItem != null)
            {
                selItem.IsSelected = value;

                var holder = GetTrackedHolder(selItem);
                holder?.RefreshViewState();
            }
        }

        public bool IsItemSelected(object item)
        {
            var selItem = _selectableItems.SafeFirstOrDefault(si => si.Item != null && si.Item.Equals(item));
            if (selItem == null)
                throw new IndexOutOfRangeException($"IsItemSelected - No item found");

            return selItem != null ? selItem.IsSelected : false;
        }

        public void BindHolder(SelectableViewHolder holder, object item, int itemPosition, bool singleSelection, bool selectionEnabled)
        {
            var selItem = _selectableItems.SafeFirstOrDefault(si => si.Item != null && si.Item.Equals(item));
            if (selItem == null)
                throw new IndexOutOfRangeException($"BindHolder - No item found");            

            holder.BindSelectableItem(selItem, singleSelection, selectionEnabled);

            if (!IsHolderTracked(holder))
                _trackedViewHolders.Add(new WeakReference<SelectableViewHolder>(holder));
        }

        /// <summary>
        /// Returns the holder of a specific selectable item. If non-null, the returned
        /// holder is guaranteed to have getPosition() == position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        private SelectableViewHolder GetTrackedHolder(SelectableItem selItem)
        {
            foreach (var weakReference in _trackedViewHolders)
            {
                SelectableViewHolder svh = null;
                if (weakReference.TryGetTarget(out svh) && svh != null)
                {
                    if(svh.SelectableItem != null && svh.SelectableItem.Equals(selItem))
                        return svh;
                }
            }

            return null;
        }
    }
}