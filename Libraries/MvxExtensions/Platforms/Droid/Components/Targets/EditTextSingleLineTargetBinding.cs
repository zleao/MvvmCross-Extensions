using Android.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace MvxExtensions.Platforms.Droid.Components.Targets
{
    /// <summary>
    /// Binding to set single or multiple line in and EditText
    /// </summary>
    public class EditTextSingleLineTargetBinding : MvxAndroidTargetBinding<EditText,bool>
    {
        public EditTextSingleLineTargetBinding(EditText editText)
            : base(editText)
        {
        }

        protected override void SetValueImpl(EditText target, bool value)
        {
            target.SetSingleLine(value);
        }
    }
}