using MvvmCross.Binding;
using MvvmCross.Platforms.Android.Binding.Target;
using MvxExtensions.Platforms.Droid.Components.Controls;

namespace MvxExtensions.Platforms.Droid.Components.Targets
{
    /// <inheritdoc/>
    public class DecimalEditTextDecimalValueTargetBinding : MvxAndroidTargetBinding<DecimalEditText,object>
    {
        /// <summary>Gets the default mode.</summary>
        /// <value>The default mode.</value>
        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        /// <summary>Initializes a new instance of the <see cref="DecimalEditTextDecimalValueTargetBinding" /> class.</summary>
        /// <param name="decimalEditText">The decimal edit text.</param>
        public DecimalEditTextDecimalValueTargetBinding(DecimalEditText decimalEditText)
            : base(decimalEditText)
        {
            Target.OnDecimalValueChanged = OnDecimalValueChanged;
        }

        /// <summary>Sets the value implementation.</summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
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
