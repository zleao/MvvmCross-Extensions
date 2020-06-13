using System;
using MvvmCross.Binding;
using MvvmCross.Exceptions;
using MvvmCross.Platforms.Android.Binding.Target;
using MvxExtensions.Core.Extensions;
using MvxExtensions.Extensions;
using MvxExtensions.Platforms.Droid.Components.Controls;

namespace MvxExtensions.Platforms.Droid.Components.Targets
{
    public class NumericEditTextIntValueTargetBinding : MvxAndroidTargetBinding<NumericEditText,object>
    {
        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public NumericEditTextIntValueTargetBinding(NumericEditText numericEditText)
            : base(numericEditText)
        {
            Target.OnIntValueChanged = OnIntValueChanged;
        }

        protected override void SetValueImpl(NumericEditText target, object value)
        {
             var step = 0;
            try
            {
                step = 1;
                if (!(value is int intValueNullable))
                {
                    if (value == null)
                    {
                        MvxBindingLog.Warning("Null value passed to NumericEditText.IntValue binding");
                    }
                    else
                    {
                        step = 2;
                        var stringValue = value.ToString();
                        if(stringValue.IsNullOrEmpty())
                            MvxBindingLog.Warning("Null value passed to NumericEditText.IntValue binding");

                        step = 3;
                        var indexOfDecimalSeparator = stringValue.IndexOf('.');
                        if(indexOfDecimalSeparator < 0)
                            indexOfDecimalSeparator = stringValue.IndexOf(',');

                        if (indexOfDecimalSeparator < 0)
                        {
                            step = 4;
                            Target.IntValue = int.Parse(stringValue);
                        }
                        else
                        {
                            step = 5;
                            Target.IntValue = int.Parse(stringValue.Substring(0, indexOfDecimalSeparator));
                        }
                    }
                    return;
                }
                else
                {
                    step = 6;
                    Target.IntValue = intValueNullable;
                }
            }
            catch (Exception ex)
            {
                MvxBindingLog.Error("NumericEditText.IntValue.SetValueImpl found an error for value ({0}) in step {1}: {2}".SafeFormatTemplate(value, step, ex.ToLongString()));
            }
        }

        private void OnIntValueChanged(int value)
        {
            FireValueChanged(value);
        }
    }
}
