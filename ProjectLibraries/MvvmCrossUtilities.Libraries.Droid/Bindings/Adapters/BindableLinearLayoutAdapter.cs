using Android.Content;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Binding.Droid.Views;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters
{
    public class BindableLinearLayoutAdapter : MvxAdapterWithChangedEvent, Android.Views.View.IOnClickListener
    {
        public delegate void ItemClickDelegate(object item);
        public ItemClickDelegate OnItemClick;

        public BindableLinearLayoutAdapter(Context context)
            : base(context)
        {
        }

        protected override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent, int templateId)
        {
            var view = base.GetView(position, convertView, parent, templateId);
            view.SetOnClickListener(this);

            return view;
        }

        #region IOnClickListener Members

        public void OnClick(Android.Views.View v)
        {
            IMvxDataConsumer dataConsumer = v as IMvxDataConsumer;

            if (dataConsumer != null && OnItemClick != null)
                OnItemClick(dataConsumer.DataContext);
        }

        #endregion
    }
}