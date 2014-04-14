using System.Collections.Specialized;
using Android.Content;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Binding.Droid.Views;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters
{
    public class SelectableListViewAdapter : MvxAdapter
    {
        public delegate void ItemsSourceChangedDelegate(NotifyCollectionChangedEventArgs args);
        public ItemsSourceChangedDelegate OnItemsSourceChanged;

        public SelectableListViewAdapter(Context context)
            : this(context, MvxAndroidBindingContextHelpers.Current())
        {
        }

        public SelectableListViewAdapter(Context context, IMvxAndroidBindingContext bindingContext)
            : base(context, bindingContext)
        {
        }

        protected override void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsSourceCollectionChanged(sender, e);

            if (OnItemsSourceChanged != null)
                OnItemsSourceChanged(e);
        }
    }
}