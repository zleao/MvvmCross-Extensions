using Android.Views;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Binding.ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.SelectableRecyclerViewComponents
{
    public class SelectableViewHolderManager
    {
        #region Fields

        protected readonly List<WeakReference<SelectableViewHolder>> _trackedViewHolders = new List<WeakReference<SelectableViewHolder>>();
        protected readonly List<SelectableItem> _selectableItems = new List<SelectableItem>();

        #endregion

        #region Methods

        public virtual void SetItemsSource(IEnumerable itemsToWrap, IList selectedItems = null)
        {
            _selectableItems.Clear();
            if (itemsToWrap.SafeCount() > 0)
            {
                foreach (var item in itemsToWrap)
                {
                    var selectableItem = new SelectableItem(item);

                    if (selectedItems != null)
                        selectableItem.IsSelected = selectedItems.Contains(item);

                    _selectableItems.Add(selectableItem);                    
                }
            }
            ExecuteOnAllTrackedHolders(h => h.Reset());
        }

        public virtual void OnItemsSourceCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            try
            {
                if (args != null)
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var item in args.NewItems)
                            {
                                _selectableItems.Add(new SelectableItem(item));
                            }
                            break;

                        case NotifyCollectionChangedAction.Move:
                            Android.Util.Log.Error("SelectableViewHolderManager.OnItemsSourceChanged", "NotifyCollectionChangedAction.Move not supported");
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            foreach (var item in args.OldItems)
                            {
                                var selItem = _selectableItems.SafeFirstOrDefault(i => i.Item != null ? i.Item.Equals(item) : false);
                                if (selItem != null)
                                    _selectableItems.RemoveAll(i => i.Item != null ? i.Item.Equals(item) : false);
                            }
                            break;

                        case NotifyCollectionChangedAction.Replace:
                            Android.Util.Log.Error("SelectableViewHolderManager.OnItemsSourceChanged", "NotifyCollectionChangedAction.Replace not supported");
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

        public virtual SelectableViewHolder CreateNewHolder(View itemView, IMvxAndroidBindingContext context)
        {
            return new SelectableViewHolder(itemView, context);
        }

        public void BindHolder(SelectableViewHolder holder, int position, bool singleSelection, bool selectionEnabled)
        {
            var item = _selectableItems.ElementAt(position) as SelectableItem;            

            holder.BindSelectableItem(item, singleSelection, selectionEnabled);

            if (!IsHolderTracked(holder))
                _trackedViewHolders.Add(new WeakReference<SelectableViewHolder>(holder));
        }

        public void SetSingleSelection(bool value)
        {
            ExecuteOnAllTrackedHolders(h => h.SetSingleSelection(value));
        }

        public void SetSelectionEnabled(bool value)
        {
            ExecuteOnAllTrackedHolders(h => h.SetSelectionEnabled(value));
        }

        public void SetItemSelection(int position, bool value)
        {
            var item = _selectableItems.ElementAt(position) as SelectableItem;
            if (item != null)
                item.IsSelected = value;

            var holder = GetTrackedHolder(position);
            if (holder != null)
                holder.RefreshViewState();
        }

        public void SetAllItemsSelection(bool value)
        {
            foreach (var item in _selectableItems)
                item.IsSelected = value;

            ExecuteOnAllTrackedHolders(h => h.RefreshViewState());
        }

        public bool IsItemSelected(int position)
        {
            var item = _selectableItems.ElementAt(position) as SelectableItem;
            if (item == null)
                throw new IndexOutOfRangeException($"No item found at position {position}");

            return item != null ? item.IsSelected : false;
        }

        protected bool IsHolderTracked(SelectableViewHolder holder)
        {
            var tvh = _trackedViewHolders.SafeFirstOrDefault(vh =>
            {
                SelectableViewHolder svh = null;
                if (vh.TryGetTarget(out svh) && svh != null && svh.Handle != IntPtr.Zero)
                {
                    return svh.Equals(holder);
                }
                    
                return false;
            });

            return tvh != null;
        }

        /// <summary>
        /// Returns the holder with a given position. If non-null, the returned
        /// holder is guaranteed to have getPosition() == position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        private SelectableViewHolder GetTrackedHolder(int position)
        {
            foreach (var weakReference in _trackedViewHolders)
            {
                SelectableViewHolder svh = null;
                if (weakReference.TryGetTarget(out svh) && svh != null && svh.Handle != IntPtr.Zero && svh.AdapterPosition == position)
                    return svh;
            }

            return null;
        }

        protected virtual void ExecuteOnAllTrackedHolders(Action<SelectableViewHolder> action)
        {
            foreach (var weakReference in _trackedViewHolders)
            {
                SelectableViewHolder svh = null;
                if (weakReference.TryGetTarget(out svh) && svh != null)
                    action.Invoke(svh);
            }
        }

        #endregion
    }
}