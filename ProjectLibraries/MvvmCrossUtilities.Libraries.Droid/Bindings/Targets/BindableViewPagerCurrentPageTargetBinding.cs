using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Droid.Target;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views;
using System;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Targets
{
    public class BindableViewPagerCurrentPageTargetBinding : MvxAndroidTargetBinding
    {
        protected BindableViewPager BindableViewPager
        {
            get { return (BindableViewPager)Target; }
        }

        public BindableViewPagerCurrentPageTargetBinding(BindableViewPager bindableViewPager)
            : base(bindableViewPager)
        {
            BindableViewPager.OnCurrentPageChanged = OnCurrentPageChanged;
        }

        protected override void SetValueImpl(object target, object value)
        {            
            if (value == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to BindableViewPager.CurrentPage binding");
                return;
            }

            if (BindableViewPager.CurrentPage != value)
                ((BindableViewPager)target).CurrentPage = value;
        }

        public override Type TargetType
        {
            get { return typeof(BindableViewPager); }
        }

        private void OnCurrentPageChanged(object page)
        {
            FireValueChanged(page);
        }
    }
}