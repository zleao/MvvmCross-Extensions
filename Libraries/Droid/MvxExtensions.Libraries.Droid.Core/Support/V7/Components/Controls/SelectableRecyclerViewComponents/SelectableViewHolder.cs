using Android.Views;
using Android.Widget;
using MvvmCross.Binding.Droid.BindingContext;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.SelectableRecyclerViewComponents
{
    public class SelectableViewHolder : BaseRecyclerViewHolder
    {
        #region Fields

        private readonly ImageView _singleImage = null;
        private readonly ImageView _multiImage = null;

        private SelectableItem _cachedSelectableItem;
        private bool _singleSelection = false;
        private bool _selectionEnabled = true;

        #endregion

        #region Properties

        public SelectableItem SelectableItem { get; private set; }

        #endregion

        #region Constructor

        public SelectableViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            _singleImage = itemView.FindViewWithTag("SingleSelectionImage") as ImageView;
            if (_singleImage != null)
                _singleImage.Visibility = ViewStates.Gone;

            _multiImage = itemView.FindViewWithTag("MultiSelectionImage") as ImageView;
            if (_multiImage != null)
                _multiImage.Visibility = ViewStates.Gone;
        }

        #endregion

        #region Methods

        public void BindSelectableItem(SelectableItem item, bool singleSelection, bool selectionEnabled)
        {
            _selectionEnabled = selectionEnabled;
            _singleSelection = singleSelection;
            UpdateImagesVisibility();

            SelectableItem = item;

            RefreshViewState();
        }

        public virtual void SetSelectionEnabled(bool value)
        {
            _selectionEnabled = value;

            UpdateImagesVisibility();
        }

        public virtual void SetSingleSelection(bool value)
        {
            _singleSelection = value;

            UpdateImagesVisibility();
        }

        protected virtual void UpdateImagesVisibility()
        {
            if (_singleImage != null)
                _singleImage.Visibility = ((_selectionEnabled && _singleSelection) ? ViewStates.Visible : ViewStates.Gone);

            if (_multiImage != null)
                _multiImage.Visibility = ((_selectionEnabled && !_singleSelection) ? ViewStates.Visible : ViewStates.Gone);
        }

        public virtual void RefreshViewState()
        {
            if (SelectableItem != null)
            {
                var checkableView = this.ItemView as ICheckable;
                if (checkableView != null && checkableView.Checked != SelectableItem.IsSelected)
                    checkableView.Toggle();
            }
        }

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (_cachedSelectableItem != null && SelectableItem == null)
                BindSelectableItem(_cachedSelectableItem, _singleSelection, _selectionEnabled);
        }

        public override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();

            _cachedSelectableItem = SelectableItem;
            SelectableItem = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _cachedSelectableItem = null;

            base.Dispose(disposing);
        }

        public virtual void Reset()
        {
            DataContext = null;
            _cachedSelectableItem = null;
        }

        #endregion
    }
}