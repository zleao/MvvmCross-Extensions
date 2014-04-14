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

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views
{
    public class BindableViewPager : Android.Support.V4.View.ViewPager
    {

        public BindableViewPager(Context context, IAttributeSet attrs)
            : this(context, attrs, new MvxBindablePagerAdapter(context))
        { }

        public BindableViewPager(Context context, IAttributeSet attrs, MvxBindablePagerAdapter adapter)
            : base(context, attrs)
        {
            var itemTemplateId = MvxAttributeHelpers.ReadListItemTemplateId(context, attrs);
            adapter.ItemTemplateId = itemTemplateId;
            Adapter = adapter;

            base.PageSelected += OnPageSelectedHandler;
        }

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

                if (value == null)
                {
                    Adapter.ItemsSource = null;
                }
                else if (value is IEnumerable)
                {
                    Adapter.ItemsSource = value as IEnumerable;
                    if (Adapter.ItemsSource.CountOrZero() > 0)
                        OnPageSelectedHandler(this, new PageSelectedEventArgs(CurrentPageIndex));
                }
                else
                {
                    var list = new List<object>();
                    list.Add(value);
                    Adapter.ItemsSource = list;
                    OnPageSelectedHandler(this, new PageSelectedEventArgs(CurrentPageIndex));
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

        public ICommand PageSelectedCommand
        {
            get { return _pageSelectedCommand; }
            set { _pageSelectedCommand = value; }
        }
        private ICommand _pageSelectedCommand;

        private void OnPageSelectedHandler(object sender, PageSelectedEventArgs args)
        {
            if (_currentPageIndex != args.Position)
            {
                _currentPageIndex = args.Position;
                if (OnCurrentPageIndexChanged != null)
                    OnCurrentPageIndexChanged(_currentPageIndex);
                ExecuteCommand(PageSelectedCommand, args.Position);
            }
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

        public override bool OnTouchEvent(Android.Views.MotionEvent e)
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

        public delegate void CurrentPageIndexChangedDelegate(int pageIndex);
        public CurrentPageIndexChangedDelegate OnCurrentPageIndexChanged;
    }
}