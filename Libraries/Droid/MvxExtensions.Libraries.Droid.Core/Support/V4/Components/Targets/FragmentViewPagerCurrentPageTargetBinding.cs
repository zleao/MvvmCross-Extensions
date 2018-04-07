using MvxExtensions.Libraries.Droid.Core.Support.V4.Components.Controls;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;
using MvvmCross.Platform.Platform;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Support.V4.Components.Targets
{
    public class FragmentViewPagerCurrentPageTargetBinding : MvxAndroidTargetBinding
    {
        protected FragmentViewPager FragmentViewPager
        {
            get { return (FragmentViewPager)Target; }
        }

        public FragmentViewPagerCurrentPageTargetBinding(FragmentViewPager bindableViewPager)
            : base(bindableViewPager)
        {
            FragmentViewPager.OnCurrentPageChanged = OnCurrentPageChanged;
        }

        protected override void SetValueImpl(object target, object value)
        {
            if (value == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to FragmentViewPager.CurrentPage binding");
                return;
            }

            if (FragmentViewPager.CurrentPage != value)
                ((FragmentViewPager)target).CurrentPage = value;
        }

        public override Type TargetType
        {
            get { return typeof(FragmentViewPager); }
        }

        private void OnCurrentPageChanged(object page)
        {
            FireValueChanged(page);
        }
    }
}