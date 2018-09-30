using Android.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace MvxExtensions.Platforms.Droid.Components.Targets
{
    /// <summary>
    /// Binding to set the text of a toggle button when its value is false
    /// </summary>
    public class ToggleButtonTextLabelOnTargetBinding : MvxAndroidTargetBinding<ToggleButton,string>
    {
        public ToggleButtonTextLabelOnTargetBinding(ToggleButton toggleButton)
            : base(toggleButton)
        {
        }

        protected override void SetValueImpl(ToggleButton target, string value)
        {
            target.TextOn = value;
            target.Checked = target.Checked;
        }
    }
}