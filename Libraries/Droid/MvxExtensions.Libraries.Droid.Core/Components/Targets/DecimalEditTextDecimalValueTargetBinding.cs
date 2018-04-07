using MvxExtensions.Libraries.Droid.Core.Components.Controls;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;
using MvvmCross.Platform.Platform;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Components.Targets
{
    public class DecimalEditTextDecimalValueTargetBinding : MvxAndroidTargetBinding
    {
        protected DecimalEditText DecimalEditTextTarget
        {
            get { return (DecimalEditText)Target; }
        }

        public DecimalEditTextDecimalValueTargetBinding(DecimalEditText decimalEditText)
            : base(decimalEditText)
        {
            DecimalEditTextTarget.OnDecimalValueChanged = OnDecimalValueChanged;
        }

        public override MvxBindingMode DefaultMode
        {
            get { return MvxBindingMode.TwoWay; }
        }

        public override Type TargetType
        {
            get { return typeof(DecimalEditText); }
        }

        protected override void SetValueImpl(object target, object value)
        {
            var decimalValue = value as decimal?;
            if (decimalValue == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to DecimalEditText.DecimalValue binding");
                return;
            }

            DecimalEditTextTarget.DecimalValue = decimalValue.Value;
        }

        private void OnDecimalValueChanged(decimal value)
        {
            FireValueChanged(value);
        }
    }
}