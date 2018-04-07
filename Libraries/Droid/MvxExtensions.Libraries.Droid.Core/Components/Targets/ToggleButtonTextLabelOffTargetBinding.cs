using Android.Widget;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;
using MvvmCross.Platform.Platform;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Components.Targets
{
    public class ToggleButtonTextLabelOffTargetBinding : MvxAndroidTargetBinding
    {
        protected ToggleButton ToggleButton
        {
            get { return (ToggleButton)Target; }
        }

        public ToggleButtonTextLabelOffTargetBinding(ToggleButton toggleButton)
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
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to ToggleButton.TextLabelOff binding");
                return;
            }

            ((ToggleButton)target).TextOff = stringValue;
            ((ToggleButton)target).Checked = ((ToggleButton)target).Checked;
        }
    }
}