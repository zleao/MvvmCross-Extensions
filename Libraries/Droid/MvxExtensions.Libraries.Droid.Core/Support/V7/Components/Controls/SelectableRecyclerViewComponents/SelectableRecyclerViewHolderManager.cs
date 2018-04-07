using Android.Util;
using Android.Views;
using MvvmCross.Binding.Droid.BindingContext;
using System;
using System.Collections.Generic;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls
{
    public class SelectableRecyclerViewHolderManager
    {
        #region Fields

        private readonly SparseArray<WeakReference<SelectableRecyclerViewHolder>> _holdersByPosition =
            new SparseArray<WeakReference<SelectableRecyclerViewHolder>>();

        private readonly SparseBooleanArray _itemsSelectionByHashCode = new SparseBooleanArray();

        #endregion

        #region Methods

        public virtual SelectableRecyclerViewHolder AddNewHolder(View itemView, IMvxAndroidBindingContext context, bool singleSelection)
        {
            return new SelectableRecyclerViewHolder(itemView, context, singleSelection, _itemsSelectionByHashCode);
        }

        public void RefreshSelectionMode(bool singleSelection)
        {
            foreach (var holder in GetTrackedHolders())
            {
                holder.SetSingleSelection(singleSelection);
            }
        }

        public bool IsSelected(int itemHashCode)
        {
            return _itemsSelectionByHashCode.Get(itemHashCode);
        }

        public void SetItemSelection(int position, bool value)
        {
            var holder = GetHolder(position);
            if (holder != null)
                holder.SetIsSelected(value);
        }

        public void SetAllItemsSelection(bool value)
        {
            for (int i = 0; i < _itemsSelectionByHashCode.Size(); i++)
            {
                int key = _itemsSelectionByHashCode.KeyAt(i);
                _itemsSelectionByHashCode.Put(key, value);
            }

            foreach (var holder in GetTrackedHolders())
                holder.RefreshViewState();
        }


        public void BindHolder(SelectableRecyclerViewHolder holder, int position, bool singleSelection)
        {
            holder.SetSingleSelection(singleSelection);
            holder.RefreshViewState();

            _holdersByPosition.Put(position, new WeakReference<SelectableRecyclerViewHolder>(holder));
        }

        /// <summary>
        /// Returns the holder with a given position. If non-null, the returned
        /// holder is guaranteed to have getPosition() == position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        private SelectableRecyclerViewHolder GetHolder(int position)
        {
            var holderRef = _holdersByPosition.Get(position);
            if (holderRef == null)
            {
                return null;
            }

            SelectableRecyclerViewHolder holder = null;
            holderRef.TryGetTarget(out holder);

            if (holder == null || holder.AdapterPosition != position)
            {
                _holdersByPosition.Remove(position);
                return null;
            }

            return holder;
        }

        private List<SelectableRecyclerViewHolder> GetTrackedHolders()
        {
            var holders = new List<SelectableRecyclerViewHolder>();

            for (int i = 0; i < _holdersByPosition.Size(); i++)
            {
                int key = _holdersByPosition.KeyAt(i);
                var holder = GetHolder(key);

                if (holder != null)
                {
                    holders.Add(holder);
                }
            }

            return holders;
        }

        #endregion
    }
}