using Android.Content;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using MvxExtensions.Libraries.Droid.Core.Extensions;
using MvxExtensions.Libraries.Portable.Core.Extensions;

namespace MvxExtensions.Libraries.Droid.Core.Components.Controls
{
    public class NumericEditText : EditText
    {
        public delegate void IntValueChangedDelegate(int value);
        public IntValueChangedDelegate OnIntValueChanged;

        public int IntValue
        {
            get { return _intValue; }
            set { SetIntValue(value, false); }
        }
        private int _intValue;

        public NumericEditText(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            var gravityInt = attrs.GetAttributeIntValue("http://schemas.android.com/apk/res/android", "gravity", -1);
            if (gravityInt == -1)
            {
                this.Gravity = GravityFlags.Right;
            }

            this.SetRawInputType(InputTypes.ClassNumber);
            this.SetSingleLine(true);
            this.WeakSubscribeTextChanged(OnTextChanged);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            int intValue = 0;

            var stringValue = (e.Text as SpannableStringBuilder).ToString();
            if (!string.IsNullOrEmpty(stringValue))
            {
                if (!int.TryParse(stringValue, out intValue))
                    Android.Util.Log.Wtf("NumericEditText.OnTextChanged", "Error trying to convert string ({0}) to int".SafeFormatTemplate(stringValue));
            }

            ChangeAndPropagateIntValue(intValue);
        }

        private void SetIntValue(int value, bool forceUpdate)
        {
            if (forceUpdate || _intValue != value || Text.IsNullOrEmpty())
            {
                _intValue = value;

                Text = value.ToString("0");
            }
        }

        private void ChangeAndPropagateIntValue(int value)
        {
            if (_intValue != value)
            {
                _intValue = value;

                if (OnIntValueChanged != null)
                    OnIntValueChanged(_intValue);
            }
        }
    }
}