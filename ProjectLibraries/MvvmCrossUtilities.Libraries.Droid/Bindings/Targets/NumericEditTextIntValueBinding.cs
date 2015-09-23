using Cirrious.CrossCore.Exceptions;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Droid.Target;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views;
using System;
using System.Globalization;
using MvvmCrossUtilities.Libraries.Portable.Extensions;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Targets
{
    public class NumericEditTextIntValueTargetBinding : MvxAndroidTargetBinding
    {
        protected NumericEditText NumericEditTextTarget
        {
            get { return (NumericEditText)Target; }
        }

        public NumericEditTextIntValueTargetBinding(NumericEditText numericEditText)
            : base(numericEditText)
        {
            NumericEditTextTarget.OnIntValueChanged = OnIntValueChanged;
        }

        public override MvxBindingMode DefaultMode
        {
            get { return MvxBindingMode.TwoWay; }
        }

        public override Type TargetType
        {
            get { return typeof(NumericEditText); }
        }

        protected override void SetValueImpl(object target, object value)
        {
            var step = 0;
            try
            {
                step = 1;
                var intValueNullable = value as int?;
                if (intValueNullable == null)
                {
                    if (value == null)
                    {
                        MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to NumericEditText.IntValue binding");
                    }
                    else
                    {
                        step = 2;
                        var stringValue = value.ToString();
                        if(stringValue.IsNullOrEmpty())
                            MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Null value passed to NumericEditText.IntValue binding");

                        step = 3;
                        var indexOfDecimalSeparator = stringValue.IndexOf('.');
                        if(indexOfDecimalSeparator < 0)
                            indexOfDecimalSeparator = stringValue.IndexOf(',');

                        if (indexOfDecimalSeparator < 0)
                        {
                            step = 4;
                            NumericEditTextTarget.IntValue = int.Parse(stringValue);
                        }
                        else
                        {
                            step = 5;
                            NumericEditTextTarget.IntValue = int.Parse(stringValue.Substring(0, indexOfDecimalSeparator));
                        }
                    }
                    return;
                }
                else
                {
                    step = 6;
                    NumericEditTextTarget.IntValue = intValueNullable.Value;
                }
            }
            catch (Exception ex)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Error, "NumericEditText.IntValue.SetValueImpl found an error for value ({0}) in step {1}: {2}".SafeFormatTemplate(value, step, ex.ToLongString()));
            }
        }

        private void OnIntValueChanged(int value)
        {
            FireValueChanged(value);
        }
    }
}