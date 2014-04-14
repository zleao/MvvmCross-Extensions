using System;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Droid.Target;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Targets
{
    public class BindableViewPagerCurrentPageIndexTargetBinding : MvxAndroidTargetBinding
    {
        protected BindableViewPager BindableViewPager
        {
            get { return (BindableViewPager)Target; }
        }

        public BindableViewPagerCurrentPageIndexTargetBinding(BindableViewPager bindableViewPager)
            : base(bindableViewPager)
        {
            BindableViewPager.OnCurrentPageIndexChanged = OnCurrentPageIndexChanged;
        }

        public override MvxBindingMode DefaultMode
        {
            get { return MvxBindingMode.TwoWay; }
        }

        public override Type TargetType
        {
            get { return typeof(BindableViewPager); }
        }

        protected override void SetValueImpl(object target, object value)
        {
            var intValue = value as int?;
            if (intValue == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to BindableViewPager.CurrentPageIndex binding");
                return;
            }

            if (BindableViewPager.CurrentPageIndex != intValue.Value)
                ((BindableViewPager)target).CurrentPageIndex = intValue.Value;
        }

        private void OnCurrentPageIndexChanged(int pageIndex)
        {
            FireValueChanged(pageIndex);
        }
    }
}