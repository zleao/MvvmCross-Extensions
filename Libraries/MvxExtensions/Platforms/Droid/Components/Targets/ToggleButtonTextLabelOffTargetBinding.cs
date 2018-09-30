using Android.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace MvxExtensions.Platforms.Droid.Components.Targets
{
    /// <summary>
    /// Binding to set the text of a toggle button when its value is false
    /// </summary>
    public class ToggleButtonTextLabelOffTargetBinding : MvxAndroidTargetBinding<ToggleButton,string>
    {
        public ToggleButtonTextLabelOffTargetBinding(ToggleButton toggleButton)
            : base(toggleButton)
        {
        }

        protected override void SetValueImpl(ToggleButton target, string value)
        {
            target.TextOff = value;
            target.Checked = target.Checked;
        }
    }
}