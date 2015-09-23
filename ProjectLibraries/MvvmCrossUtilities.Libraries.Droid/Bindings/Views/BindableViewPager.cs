//All credits to:
//
// Derivative of BindableViewPager.cs by Steve Dunford
// http://slodge.blogspot.com/2013/02/binding-to-androids-horizontal-pager.html
// which is a derivative of MvxBindableListView.cs from the
// MvvmCross project by Stuart Lodge
// https://github.com/slodge/MvvmCross/blob/vnext/Cirrious/Cirrious.MvvmCross.Binding.Droid/Views/MvxBindableListView.cs
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.
// 
// Project Lead - Tomasz Cielecli, @Cheesebaron, tomasz@ostebaronen.dk

using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Android.Content;
using Android.Util;
using Cirrious.MvvmCross.Binding.Attributes;
using Cirrious.MvvmCross.Binding.Droid.Views;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters;
using MvvmCrossUtilities.Libraries.Portable.Extensions;
using Cirrious.MvvmCross.Binding.ExtensionMethods;
using Android.Views;
using Android.Support.V4.View;
using System;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views
{
    public class BindableViewPager : ViewPager
    {
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

        public new MvxBindablePagerAdapter Adapter
        {
            get { return base.Adapter as MvxBindablePagerAdapter; }
            set
            {
                var existing = Adapter;
                if (existing == value)
                    return;

                if (existing != null && value != null)
                {
                    value.ItemsSource = existing.ItemsSource;
                    value.ItemTemplateId = existing.ItemTemplateId;
                }

                base.Adapter = value;
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

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
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

        public BindableViewPager(Context context, IAttributeSet attrs)
            : this(context, attrs, new MvxBindablePagerAdapter(context))
        { }

        public BindableViewPager(Context context, IAttributeSet attrs, MvxBindablePagerAdapter adapter)
            : base(context, attrs)
        {
            var itemTemplateId = MvxAttributeHelpers.ReadListItemTemplateId(context, attrs);
            adapter.ItemTemplateId = itemTemplateId;
            Adapter = adapter;


            PageSelected += OnPageSelectedHandler;
        }

        ~BindableViewPager()
        {
            Dispose(true);
        }
        #endregion

        #region Methods

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

            _currentPage = Adapter.ItemsSource.ElementAt(_currentPageIndex);

            if (OnCurrentPageChanged != null)
                OnCurrentPageChanged(_currentPage);
        }

        protected virtual void ExecuteCommand(ICommand command, int toPage)
        {
            if (command == null)
                return;

            if (!command.CanExecute(toPage))
                return;

            command.Execute(toPage);
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
                base.PageSelected -= OnPageSelectedHandler;
            }
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();

            _ignoreCurrentIndex = true;
            OnPageSelectedHandler(this, new PageSelectedEventArgs(CurrentItem));
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