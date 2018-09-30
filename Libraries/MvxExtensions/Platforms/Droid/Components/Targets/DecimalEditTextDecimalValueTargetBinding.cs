using MvvmCross.Binding;
using MvvmCross.Platforms.Android.Binding.Target;
using MvxExtensions.Platforms.Droid.Components.Controls;

namespace MvxExtensions.Platforms.Droid.Components.Targets
{
    public class DecimalEditTextDecimalValueTargetBinding : MvxAndroidTargetBinding<DecimalEditText,object>
    {
        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;
        
        public DecimalEditTextDecimalValueTargetBinding(DecimalEditText decimalEditText)
            : base(decimalEditText)
        {
            Target.OnDecimalValueChanged = OnDecimalValueChanged;
        }

        protected override void SetValueImpl(DecimalEditText target, object value)
        {
            if (!(value is decimal decimalValue))
            {
                MvxBindingLog.Warning("Null value passed to DecimalEditText.DecimalValue binding");
                return;
            }

            Target.DecimalValue = decimalValue;
        }

        private void OnDecimalValueChanged(decimal value)
        {
            FireValueChanged(value);
        }
    }
}
