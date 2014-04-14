using System.Windows.Input;
using Android.Content;
using Android.Util;
using Cirrious.MvvmCross.Binding.Droid.Views;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views
{
    public class BindableLinearLayout : MvxLinearLayout
    {
        #region Commands

        public ICommand ItemClick { get; set; }

        #endregion

        #region Constructor

        public BindableLinearLayout(Context context, IAttributeSet attrs)
            : this(context, attrs, new BindableLinearLayoutAdapter(context))
        {

        }

        public BindableLinearLayout(Context context, IAttributeSet attrs, BindableLinearLayoutAdapter adapter)
            : base(context, attrs, adapter)
        {
            (Adapter as BindableLinearLayoutAdapter).OnItemClick = OnItemClick;
        }

        #endregion

        #region Methods

        public void OnItemClick(object item)
        {
            var canSelectItem = true;

            if (ItemClick != null)
                canSelectItem = ItemClick.CanExecute(item);

            if (canSelectItem && ItemClick != null)
            {
                ItemClick.Execute(item);
            }
        }

        #endregion
    }
}