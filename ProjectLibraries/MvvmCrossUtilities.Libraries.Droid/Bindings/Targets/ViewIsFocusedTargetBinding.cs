using System;
using Android.Views;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Droid.Target;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Targets
{
    public class ViewIsFocusedTargetBinding : MvxAndroidTargetBinding
    {
        protected View View
        {
            get { return (View)Target; }
        }

        public ViewIsFocusedTargetBinding(View view)
            : base(view)
        {
        }

        public override MvxBindingMode DefaultMode
        {
            get { return MvxBindingMode.OneWay; }
        }

        public override Type TargetType
        {
            get { return typeof(bool); }
        }

        protected override void SetValueImpl(object target, object value)
        {
            var isFocused = value as bool?;
            if (isFocused == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to View.IsFocused binding");
                return;
            }

            if (isFocused.Value && View != null)
            {
                View.RequestFocus();
            }
        }
    }
}