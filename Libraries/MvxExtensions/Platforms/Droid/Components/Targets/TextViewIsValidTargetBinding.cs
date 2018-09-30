using Android.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace MvxExtensions.Platforms.Droid.Components.Targets
{
    /// <summary>
    /// Binding to set a TextView as valid or invalid, without any text hint
    /// </summary>
    public class TextViewIsValidTargetBinding : MvxAndroidTargetBinding<TextView,bool>
    {
        public TextViewIsValidTargetBinding(TextView textView)
            : base(textView)
        {
        }

        protected override void SetValueImpl(TextView target, bool value)
        {
            target.Error = value ? null : string.Empty;
        }
    }
}