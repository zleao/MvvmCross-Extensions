using Android.OS;
using Android.Support.V4.App;
using Java.Lang;
using MvvmCross.Binding;
using MvvmCross.Binding.Attributes;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Binding.ExtensionMethods;
using MvvmCross.Platform.Exceptions;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform.WeakSubscription;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace MvxExtensions.Libraries.Droid.Core.Support.V4.Components.Adapters
{
    public class FragmentViewPagerAdapter : FragmentPagerAdapter, IPagerAdapter
    {
        public event EventHandler OnAfterDataSetChanged;

        #region Fields

        private IDisposable _subscription;

        #endregion

        #region Properties

        protected IMvxAndroidBindingContext BindingContext
        {
            get { return _bindingContext; }
        }
        private readonly IMvxAndroidBindingContext _bindingContext;


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

        [MvxSetToNullAfterBinding]
        public IEnumerable ItemsSource
        {
            get { return _itemsSource; }
            set { SetItemsSource(value); }
        }
        private IEnumerable _itemsSource;

        public override int Count => _itemsSource.Count();


        public int SimpleViewLayoutId { get; set; }

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
        private int _itemTemplateId;

        #endregion

        #region Constructor

        public FragmentViewPagerAdapter(Android.Support.V4.App.FragmentManager fragmentManager, int itemTemplateId)
            : this(fragmentManager, itemTemplateId, MvxAndroidBindingContextHelpers.Current())
        {
        }

        public FragmentViewPagerAdapter(Android.Support.V4.App.FragmentManager fragmentManager, int itemTemplateId, IMvxAndroidBindingContext bindingContext)
            : base(fragmentManager)
        {
            _bindingContext = bindingContext;
            if (_bindingContext == null)
                throw new MvxException("FragmentViewPagerAdapter can only be used within a Context which supports IMvxBindingActivity");

            SimpleViewLayoutId = Android.Resource.Layout.SimpleListItem1;
            _itemTemplateId = itemTemplateId;
        }

        #endregion

        #region Methods

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

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();

            if (OnAfterDataSetChanged != null)
                OnAfterDataSetChanged.Invoke(this, new EventArgs());
        }


        public int GetPosition(object item)
        {
            return _itemsSource.GetPosition(item);
        }

        public object GetRawItem(int position)
        {
            return _itemsSource.ElementAt(position);
        }


        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return new ViewPagerFragment(BindingContext, ItemTemplateId, GetRawItem(position));
        }

        public override ICharSequence GetPageTitleFormatted(int position)
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

        public override void RestoreState(IParcelable state, ClassLoader loader)
        {
            //Don't call restore to prevent crash on rotation
            //base.RestoreState (state, loader);
        }

        public override int GetItemPosition(Java.Lang.Object @object)
        {
            return PositionNone;
        }

        #endregion
    }
}