using Android.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace MvxExtensions.Platforms.Droid.Components.Targets
{
    /// <summary>
    /// Binding to set the max length in a TextView
    /// </summary>
    public class TextViewMaxLengthTargetBinding : MvxAndroidTargetBinding<TextView,int>
    {
        public TextViewMaxLengthTargetBinding(TextView textView)
            : base(textView)
        {

        }

        protected override void SetValueImpl(TextView target, int value)
        {
            target.SetFilters(new Android.Text.IInputFilter[] { new Android.Text.InputFilterLengthFilter(value) });
        }
    }
}
