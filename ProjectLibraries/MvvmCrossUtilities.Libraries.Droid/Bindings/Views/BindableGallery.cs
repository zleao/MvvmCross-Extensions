using System.Collections;
using Android.Content;
using Android.Util;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Attributes;
using Cirrious.MvvmCross.Binding.Droid.Views;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views
{
    public class BindableGallery : Gallery
    {
        #region Properties

        protected MvxAdapter TypedAdapter
        {
            get { return Adapter as MvxAdapter; }
        }

        [MvxSetToNullAfterBinding]
        public virtual IEnumerable ItemsSource
        {
            get { return TypedAdapter.ItemsSource; }
            set { TypedAdapter.ItemsSource = value; }
        }

        #endregion

        #region Constructor

        public BindableGallery(Context context, IAttributeSet attrs)
            : this(context, attrs, new MvxAdapter(context))
        { }

        public BindableGallery(Context context, IAttributeSet attrs, MvxAdapter adapter)
            : base(context, attrs)
        {
            var itemTemplateId = MvxAttributeHelpers.ReadListItemTemplateId(context, attrs);
            adapter.ItemTemplateId = itemTemplateId;

            Adapter = adapter;
        }

        #endregion
    }
}