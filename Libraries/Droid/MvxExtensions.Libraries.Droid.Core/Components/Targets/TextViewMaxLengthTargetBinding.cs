using Android.Widget;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;
using MvvmCross.Platform.Platform;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Components.Targets
{
    public class TextViewMaxLengthTargetBinding : MvxAndroidTargetBinding
    {
        public TextViewMaxLengthTargetBinding(TextView textView)
            : base(textView)
        {

        }

        public override MvxBindingMode DefaultMode
        {
            get { return MvxBindingMode.OneWay; }
        }

        public override Type TargetType
        {
            get { return typeof(int); }
        }

        protected override void SetValueImpl(object target, object value)
        {
            var intValue = value as int?;
            if(!intValue.HasValue)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to TextView.MaxLength binding");
                return;
            }

            ((TextView)target).SetFilters(new Android.Text.IInputFilter[] { new Android.Text.InputFilterLengthFilter(intValue.Value) });
        }
    }
}
