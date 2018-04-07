using Android.Util;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls
{
    public class SelectableRecyclerViewHolder : MvxRecyclerViewHolder
    {
        #region Fields

        private readonly ImageView _singleImage = null;
        private readonly ImageView _multiImage = null;
        private readonly SparseBooleanArray _itemsSelectionByHashCode;

        #endregion

        #region Properties

        public int DataContextHashCode
        {
            get { return DataContext != null ? DataContext.GetHashCode() : -1; }
        }

        #endregion

        #region Constructor

        public SelectableRecyclerViewHolder(View itemView, IMvxAndroidBindingContext context, bool singleSelection, SparseBooleanArray itemsSelectionByHashCode)
            : base(itemView, context)
        {
            _singleImage = itemView.FindViewWithTag("SingleSelectionImage") as ImageView;
            _multiImage = itemView.FindViewWithTag("MultiSelectionImage") as ImageView;

            _itemsSelectionByHashCode = itemsSelectionByHashCode;

            SetSingleSelection(singleSelection);
            SetIsSelected(false);
        }

        #endregion

        #region Methods

        public void SetSingleSelection(bool value)
        {
            if (_singleImage != null)
                _singleImage.Visibility = (value ? ViewStates.Visible : ViewStates.Gone);

            if (_multiImage != null)
                _multiImage.Visibility = (value ? ViewStates.Gone : ViewStates.Visible);
        }

        public void SetIsSelected(bool isSelected)
        {
            if(DataContext != null)
                _itemsSelectionByHashCode.Put(DataContext.GetHashCode(), isSelected);

            RefreshViewState(isSelected);
        }

        public void RefreshViewState()
        {
            if (DataContext != null)
            {
                var isSelected = _itemsSelectionByHashCode.Get(DataContext.GetHashCode());
                RefreshViewState(isSelected);
            }
        }

        private void RefreshViewState(bool isSelected)
        {
            var checkableView = this.ItemView as ICheckable;
            if (checkableView != null && checkableView.Checked != isSelected)
                checkableView.Toggle();
        }

        #endregion
    }
}