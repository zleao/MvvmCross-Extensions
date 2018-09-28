using Android.Views;
using MvvmCross.Platforms.Android.Binding.Target;

namespace MvxExtensions.Platforms.Droid.Components.Targets
{
    /// <summary>
    /// Binding that tries to set the focus on a view
    /// </summary>
    public class ViewIsFocusedTargetBinding : MvxAndroidTargetBinding<View,bool>
    {
        
        public ViewIsFocusedTargetBinding(View view)
            : base(view)
        {
        }

        protected override void SetValueImpl(View target, bool value)
        {
            if (value)
            {
                Target?.RequestFocus();
            }
        }
    }
}