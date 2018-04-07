using Android.Content;
using Android.Util;
using MvxExtensions.Libraries.Droid.Core.Components.Adapters;
using MvvmCross.Binding.Droid.Views;
using System.Windows.Input;

namespace MvxExtensions.Libraries.Droid.Core.Components.Controls
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
            if(ItemClick != null && ItemClick.CanExecute(item))
                ItemClick.Execute(item);
        }

        #endregion
    }
}