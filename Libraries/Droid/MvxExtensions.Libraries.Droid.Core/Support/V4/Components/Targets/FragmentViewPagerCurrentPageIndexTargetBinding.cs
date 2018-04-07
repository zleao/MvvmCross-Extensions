using MvxExtensions.Libraries.Droid.Core.Support.V4.Components.Controls;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;
using MvvmCross.Platform.Platform;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Support.V4.Components.Targets
{
    public class FragmentViewPagerCurrentPageIndexTargetBinding : MvxAndroidTargetBinding
    {
        protected FragmentViewPager FragmentViewPager
        {
            get { return (FragmentViewPager)Target; }
        }

        public FragmentViewPagerCurrentPageIndexTargetBinding(FragmentViewPager bindableViewPager)
            : base(bindableViewPager)
        {
            FragmentViewPager.OnCurrentPageIndexChanged = OnCurrentPageIndexChanged;
        }

        public override MvxBindingMode DefaultMode
        {
            get { return MvxBindingMode.TwoWay; }
        }

        public override Type TargetType
        {
            get { return typeof(FragmentViewPager); }
        }

        protected override void SetValueImpl(object target, object value)
        {
            var intValue = value as int?;
            if (intValue == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to FragmentViewPager.CurrentPageIndex binding");
                return;
            }

            if (FragmentViewPager.CurrentPageIndex != intValue.Value)
                ((FragmentViewPager)target).CurrentPageIndex = intValue.Value;
        }

        private void OnCurrentPageIndexChanged(int pageIndex)
        {
            FireValueChanged(pageIndex);
        }
    }
}