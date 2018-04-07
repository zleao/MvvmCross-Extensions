using Android.Widget;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;
using MvvmCross.Platform.Platform;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Components.Targets
{
    public class EditTextSingleLineTargetBinding : MvxAndroidTargetBinding
    {
        protected EditText EditText
        {
            get { return (EditText)Target; }
        }

        public EditTextSingleLineTargetBinding(EditText editText)
            : base(editText)
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
            var boolValue = value as bool?;
            if (boolValue == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to EditText.SingleLine binding");
                return;
            }

            ((EditText)target).SetSingleLine(boolValue.Value);
        }
    }
}