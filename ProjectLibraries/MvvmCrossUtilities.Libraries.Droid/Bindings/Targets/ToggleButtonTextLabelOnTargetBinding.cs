using System;
using Android.Widget;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Droid.Target;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Targets
{
    public class ToggleButtonTextLabelOnTargetBinding : MvxAndroidTargetBinding
    {
        protected ToggleButton ToggleButton
        {
            get { return (ToggleButton)Target; }
        }

        public ToggleButtonTextLabelOnTargetBinding(ToggleButton toggleButton)
            : base(toggleButton)
        {
        }

        public override MvxBindingMode DefaultMode
        {
            get { return MvxBindingMode.OneWay; }
        }

        public override Type TargetType
        {
            get { return typeof(string); }
        }

        protected override void SetValueImpl(object target, object value)
        {
            var stringValue = value as string;
            if (stringValue == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to ToggleButton.TextLabelOn binding");
                return;
            }

            ((ToggleButton)target).TextOn = stringValue;
            ((ToggleButton)target).Checked = ((ToggleButton)target).Checked;
        }
    }
}