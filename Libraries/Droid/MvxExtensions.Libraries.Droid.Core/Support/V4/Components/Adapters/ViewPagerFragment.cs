using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Droid.Support.V4;

namespace MvxExtensions.Libraries.Droid.Core.Support.V4.Components.Adapters
{
    public class ViewPagerFragment : MvxFragment
    {
        #region Fields

        private readonly int _itemTemplateId;

        private readonly IMvxAndroidBindingContext _androidBindingContext;

        #endregion

        #region Constructor

        public ViewPagerFragment()
            : base()
        {
        }

        public ViewPagerFragment(IMvxAndroidBindingContext androidBindingContext, int itemTemplateId, object source)
            : base()
        {
            Arguments = new Bundle();

            _androidBindingContext = androidBindingContext;
            _itemTemplateId = itemTemplateId;
            DataContext = source;
        }

        #endregion

        #region Methods

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            BindingContext = new MvxAndroidBindingContext(Context, _androidBindingContext?.LayoutInflaterHolder, DataContext);

            base.OnCreateView(inflater, container, savedInstanceState);

            return GetBindableView(DataContext);
        }

        protected virtual View GetBindableView(object source)
        {
            return GetBindableView(source, _itemTemplateId);
        }

        protected virtual View GetBindableView(object source, int templateId)
        {
            if (templateId == 0)
            {
                // no template seen - so use a standard string view from Android and use ToString()
                return GetSimpleView(source);
            }

            // we have a templateid so lets use bind and inflate on it :)
            var viewToUse = CreateBindableView(source, templateId);

            return viewToUse as View;
        }

        protected virtual View GetSimpleView(object source)
        {
            return CreateSimpleView(source);
        }

        protected virtual View CreateSimpleView(object source)
        {
            var view = ((IMvxAndroidBindingContext)BindingContext).BindingInflate(Android.Resource.Layout.SimpleListItem1, null);
            BindSimpleView(view, source);
            return view;
        }

        protected virtual void BindSimpleView(View convertView, object source)
        {
            var textView = convertView as TextView;
            if (textView != null)
            {
                textView.Text = (source ?? "").ToString();
            }
        }

        protected virtual MvxListItemView CreateBindableView(object dataContext, int templateId)
        {
            return new MvxListItemView(this.Context, ((IMvxAndroidBindingContext)BindingContext).LayoutInflaterHolder, dataContext, templateId);
        }

        #endregion
    }
}