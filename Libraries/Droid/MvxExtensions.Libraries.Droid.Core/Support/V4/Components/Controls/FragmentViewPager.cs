using Android.Content;
using Android.Content.Res;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using MvxExtensions.Libraries.Droid.Core.Support.V4.Components.Adapters;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using MvvmCross.Binding;
using MvvmCross.Binding.Attributes;
using MvvmCross.Binding.Droid.ResourceHelpers;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Binding.ExtensionMethods;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace MvxExtensions.Libraries.Droid.Core.Support.V4.Components.Controls
{
    public class FragmentViewPager : ViewPager
    {
        protected enum AdapterTypeEnum
        {
            CustomAdapter = -1,
            StateAdapter = 0,
            NormalAdapter = 1
        }

        #region Default Values

        protected virtual int DEFAULT_ADAPTER_TYPE { get { return (int)AdapterTypeEnum.StateAdapter; } }

        #endregion

        #region Fields

        private bool _ignoreCurrentIndex = false;

        #endregion

        #region Properties
        public string TitlePropertyName
        {
            get { return Adapter.TitlePropertyName; }
            set { Adapter.TitlePropertyName = value; }
        }

        public bool BlockSwipe { get; set; }

        protected AdapterTypeEnum AdapterType { get; set; }

        public new virtual IPagerAdapter Adapter
        {
            get { return base.Adapter as IPagerAdapter; }
            set
            {
                var existing = Adapter;
                if (existing == value)
                    return;

                if (existing != null && value != null)
                {
                    value.ItemsSource = existing.ItemsSource;
                    //value.ItemTemplateId = existing.ItemTemplateId;
                }

                base.Adapter = value as PagerAdapter;
            }
        }

        [MvxSetToNullAfterBinding]
        public object ItemsSource
        {
            get { return Adapter.ItemsSource; }
            set
            {
                _currentPageIndex = 0;
                _currentPage = null;

                if (value == null)
                {
                    Adapter.ItemsSource = null;
                }
                else if (value is IEnumerable)
                {
                    Adapter.ItemsSource = value as IEnumerable;
                    if (Adapter.ItemsSource.SafeCount() > 0)
                        ChangeSelectedPage(CurrentPageIndex);
                }
                else
                {
                    var list = new List<object>();
                    list.Add(value);
                    Adapter.ItemsSource = list;
                    ChangeSelectedPage(CurrentPageIndex);
                }
            }
        }

        public int ItemTemplateId
        {
            get { return Adapter.ItemTemplateId; }
            set { Adapter.ItemTemplateId = value; }
        }

        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                if (_currentPageIndex != value)
                    this.SetCurrentItem(value, true);
            }
        }
        private int _currentPageIndex;

        public object CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage != value)
                {
                    var pageIndex = Adapter.GetPosition(value);
                    this.SetCurrentItem(pageIndex < 0 ? 0 : pageIndex, true);
                }
            }
        }
        private object _currentPage;

        public ICommand PageSelectedCommand
        {
            get { return _pageSelectedCommand; }
            set { _pageSelectedCommand = value; }
        }
        private ICommand _pageSelectedCommand;

        #endregion

        #region Constructor

        public FragmentViewPager(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            ParseAttributes(context, attrs);

            var fa = context as Android.Support.V4.App.FragmentActivity;
            if (fa == null)
                throw new NotSupportedException("FragmentViewPager.Context must be a FragmentActivity");

            if (AdapterType == AdapterTypeEnum.StateAdapter)
            {
                var itemTemplateId = MvxAttributeHelpers.ReadListItemTemplateId(context, attrs);
                Adapter = new FragmentViewPagerStateAdapter(fa.SupportFragmentManager, itemTemplateId);
            }
            else if (AdapterType == AdapterTypeEnum.NormalAdapter)
            {
                var itemTemplateId = MvxAttributeHelpers.ReadListItemTemplateId(context, attrs);
                Adapter = new FragmentViewPagerAdapter(fa.SupportFragmentManager, itemTemplateId);
            }

            PageSelected += OnPageSelectedHandler;
        }

        ~FragmentViewPager()
        {
            Dispose(true);
        }
        #endregion

        #region Methods

        private void ParseAttributes(Context context, IAttributeSet attrs)
        {
            if (attrs == null)
                throw new ArgumentNullException("attrs");

            var finder = Mvx.Resolve<IMvxAppResourceTypeFinder>();
            var resourceType = finder.Find();
            var styleable = resourceType.GetNestedType("Styleable");

            var dragSortListViewAttrsId = (int[])SafeGetFieldValue(styleable, "FragmentViewPager", new int[0]);
            TypedArray a = context.ObtainStyledAttributes(attrs, dragSortListViewAttrsId, 0, 0);

            AdapterType = (AdapterTypeEnum)a.GetInt((int)SafeGetFieldValue(styleable, "FragmentViewPager_AdapterType", 0), DEFAULT_ADAPTER_TYPE);

            a.Recycle();
        }
        private object SafeGetFieldValue(Type styleable, string fieldName)
        {
            return SafeGetFieldValue(styleable, fieldName, 0);
        }
        private object SafeGetFieldValue(Type styleable, string fieldName, object defaultValue)
        {
            var field = styleable.GetField(fieldName);
            if (field == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Error, "Missing stylable field {0}", fieldName);
                return defaultValue;
            }

            return field.GetValue(null);
        }

        private void OnPageSelectedHandler(object sender, PageSelectedEventArgs args)
        {
            if (_ignoreCurrentIndex || _currentPageIndex != args.Position)
            {
                _ignoreCurrentIndex = false;

                ChangeSelectedPage(args.Position);

                ExecuteCommand(PageSelectedCommand, args.Position);
            }
        }

        private void ChangeSelectedPage(int pageIndex)
        {
            _currentPageIndex = pageIndex;

            if (OnCurrentPageIndexChanged != null)
                OnCurrentPageIndexChanged(_currentPageIndex);

            if (_currentPageIndex >= 0 && _currentPageIndex < Adapter.Count)
                _currentPage = Adapter.ItemsSource.ElementAt(_currentPageIndex);
            else
                _currentPage = null;

            if (OnCurrentPageChanged != null)
                OnCurrentPageChanged(_currentPage);
        }


        public override bool OnInterceptTouchEvent(Android.Views.MotionEvent ev)
        {
            return BlockSwipe ? false : base.OnInterceptTouchEvent(ev);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return BlockSwipe ? false : base.OnTouchEvent(e);
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {                
                if(Handle != IntPtr.Zero)
                    base.PageSelected -= OnPageSelectedHandler;
            }
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();

            _ignoreCurrentIndex = true;
            OnPageSelectedHandler(this, new PageSelectedEventArgs(CurrentItem));
        }


        protected virtual void ExecuteCommand(ICommand command, int toPage)
        {
            if (command == null)
                return;

            if (!command.CanExecute(toPage))
                return;

            command.Execute(toPage);
        }

        #endregion

        #region Delegates

        public delegate void CurrentPageIndexChangedDelegate(int pageIndex);
        public CurrentPageIndexChangedDelegate OnCurrentPageIndexChanged;

        public delegate void CurrentPageChangedDelegate(object page);
        public CurrentPageChangedDelegate OnCurrentPageChanged;

        #endregion
    }
}