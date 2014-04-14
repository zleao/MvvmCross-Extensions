//All credits to:
//
// Derivative of MvxBindablePagerAdapter.cs by Steve Dunford
// http://slodge.blogspot.com/2013/02/binding-to-androids-horizontal-pager.html
// which is a derivative of MvxBindableListAdapter.cs from the
// MvvmCross project by Stuart Lodge
// https://github.com/slodge/MvvmCross/blob/vnext/Cirrious/Cirrious.MvvmCross.Binding.Droid/Views/MvxBindableListAdapter.cs
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.
// 
// Project Lead - Tomasz Cielecli, @Cheesebaron, tomasz@ostebaronen.dk

using System;
using System.Collections;
using System.Collections.Specialized;
using Android.Content;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore.Exceptions;
using Cirrious.CrossCore.Platform;
using Cirrious.CrossCore.WeakSubscription;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Attributes;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Cirrious.MvvmCross.Binding.ExtensionMethods;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters
{
    public class MvxBindablePagerAdapter
        : PagerAdapter
    {
        private readonly IMvxAndroidBindingContext _bindingContext;
        private readonly Context _context;
        private int _itemTemplateId;
        private IEnumerable _itemsSource;
        private IDisposable _subscription;

        public MvxBindablePagerAdapter(Context context)
            : this(context, MvxAndroidBindingContextHelpers.Current())
        {
        }

        public MvxBindablePagerAdapter(Context context, IMvxAndroidBindingContext bindingContext)
        {
            _context = context;
            _bindingContext = bindingContext;
            if (_bindingContext == null)
                throw new MvxException(
                    "MvxBindableListView can only be used within a Context which supports IMvxBindingActivity");
            SimpleViewLayoutId = Android.Resource.Layout.SimpleListItem1;
        }

        protected Context Context
        {
            get { return _context; }
        }

        protected IMvxAndroidBindingContext BindingContext
        {
            get { return _bindingContext; }
        }

        public int SimpleViewLayoutId { get; set; }

        [MvxSetToNullAfterBinding]
        public IEnumerable ItemsSource
        {
            get { return _itemsSource; }
            set { SetItemsSource(value); }
        }

        public int ItemTemplateId
        {
            get { return _itemTemplateId; }
            set
            {
                if (_itemTemplateId == value)
                    return;
                _itemTemplateId = value;

                // since the template has changed then let's force the list to redisplay by firing NotifyDataSetChanged()
                if (_itemsSource != null)
                    NotifyDataSetChanged();
            }
        }

        public override int Count
        {
            get
            {
                return _itemsSource.Count();
            }
        }

        protected virtual void SetItemsSource(IEnumerable value)
        {
            if (_itemsSource == value)
                return;

            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }

            _itemsSource = value;
            if (_itemsSource != null && !(_itemsSource is IList))
                MvxBindingTrace.Trace(MvxTraceLevel.Warning,
                                      "Binding to IEnumerable rather than IList - this can be inefficient, especially for large lists");
            var newObservable = _itemsSource as INotifyCollectionChanged;
            if (newObservable != null)
                _subscription = newObservable.WeakSubscribe(OnItemsSourceCollectionChanged);
            NotifyDataSetChanged();
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyDataSetChanged(e);
        }

        public virtual void NotifyDataSetChanged(NotifyCollectionChangedEventArgs e)
        {
            base.NotifyDataSetChanged();
        }

        public int GetPosition(object item)
        {
            return _itemsSource.GetPosition(item);
        }

        public System.Object GetRawItem(int position)
        {
            return _itemsSource.ElementAt(position);
        }

        private View GetView(int position, View convertView, ViewGroup parent, int templateId)
        {
            if (_itemsSource == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Error, "GetView called when ItemsSource is null");
                return null;
            }

            var source = GetRawItem(position);

            return GetBindableView(convertView, source, templateId);
        }

        protected virtual View GetSimpleView(View convertView, object source)
        {
            if (convertView == null)
            {
                convertView = CreateSimpleView(source);
            }
            else
            {
                BindSimpleView(convertView, source);
            }

            return convertView;
        }

        protected virtual void BindSimpleView(View convertView, object source)
        {
            var textView = convertView as TextView;
            if (textView != null)
            {
                textView.Text = (source ?? "").ToString();
            }
        }

        protected virtual View CreateSimpleView(object source)
        {
            var view = _bindingContext.LayoutInflater.LayoutInflater.Inflate(SimpleViewLayoutId, null);
            BindSimpleView(view, source);
            return view;
        }

        protected virtual View GetBindableView(View convertView, object source)
        {
            return GetBindableView(convertView, source, ItemTemplateId);
        }

        protected virtual View GetBindableView(View convertView, object source, int templateId)
        {
            if (templateId == 0)
            {
                // no template seen - so use a standard string view from Android and use ToString()
                return GetSimpleView(convertView, source);
            }

            // we have a templateid so lets use bind and inflate on it :)
            var viewToUse = convertView as IMvxListItemView;
            if (viewToUse != null)
                if (viewToUse.TemplateId != templateId)
                    viewToUse = null;

            if (viewToUse == null)
                viewToUse = CreateBindableView(source, templateId);
            else
                BindBindableView(source, viewToUse);

            return viewToUse as View;
        }

        protected virtual void BindBindableView(object source, IMvxListItemView viewToUse)
        {
            viewToUse.DataContext = source;
        }

        protected virtual MvxListItemView CreateBindableView(object dataContext, int templateId)
        {
            return new MvxListItemView(_context, _bindingContext.LayoutInflater, dataContext, templateId);
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            var view = GetView(position, null, container, ItemTemplateId > 0 ? ItemTemplateId : 0);

            container.AddView(view);

            return view;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj)
        {
            var view = (View)obj;
            container.RemoveView(view);
            view.Dispose();
        }

        public override bool IsViewFromObject(View p0, Java.Lang.Object p1)
        {
            return p0 == p1;
        }

        public string TitlePropertyName
        {
            get { return _titlePropertyName; }
            set { _titlePropertyName = value; }
        }
        private string _titlePropertyName = string.Empty;

        protected string InnerTitlePropertyName
        {
            get
            {
                string propertyName = "";

                if (!string.IsNullOrEmpty(TitlePropertyName))
                    propertyName = this.TitlePropertyName;
                else
                    propertyName = "PageTitle";

                return propertyName;
            }
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            string title = string.Empty;

            if (ItemsSource.Count() > position)
            {
                var currentItem = ItemsSource.ElementAt(position);
                if (currentItem != null)
                {
                    var property = currentItem.GetType().GetProperty(InnerTitlePropertyName);
                    if (property != null)
                    {
                        var value = property.GetValue(currentItem, null);
                        title = (value != null ? value.ToString() : string.Empty);
                    }
                }
            }

            return new Java.Lang.String(title);
        }

        public override int GetItemPosition(Java.Lang.Object @object)
        {
            return PositionNone;
        }
    }
}