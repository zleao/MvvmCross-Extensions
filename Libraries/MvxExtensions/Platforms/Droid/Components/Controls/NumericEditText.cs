using System.Net.Mime;
using System.Runtime.Remoting.Contexts;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using MvxExtensions.Extensions;
using MvxExtensions.Platforms.Droid.Extensions;
using Context = Android.Content.Context;

namespace MvxExtensions.Platforms.Droid.Components.Controls
{
    [Register("mvxextensions.platforms.droid.components.controls.NumericEditText")]
    public sealed class NumericEditText : EditText
    {
        public delegate void IntValueChangedDelegate(int value);
        public IntValueChangedDelegate OnIntValueChanged;
        
        #region Properties

        private int _intValue;
        public int IntValue
        {
            get => _intValue;
            set => SetIntValue(value, false);
        }

        #endregion
        
        #region Constructor

        public NumericEditText(Context context)
            : base(context)
        {
            Initialize(null);
        }

        public NumericEditText(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize(attrs);
        }

        public NumericEditText(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Initialize(attrs);
        }

        public NumericEditText(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize(attrs);
        }

        #endregion

        #region Methods

        private void Initialize(IAttributeSet attrs)
        {
            var gravityInt = attrs?.GetAttributeIntValue("http://schemas.android.com/apk/res/android", "gravity", -1);
            if (gravityInt == null || gravityInt == -1)
            {
                Gravity = GravityFlags.Right;
            }

            KeyListener = DigitsKeyListener.GetInstance(Locale.Default, true, false);
            SetRawInputType(InputTypes.ClassNumber);
            SetSingleLine(true);
            this.WeakSubscribeTextChanged(OnTextChanged);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            int intValue = 0;

            var stringValue = (e.Text as SpannableStringBuilder)?.ToString();
            if (!string.IsNullOrEmpty(stringValue))
            {
                if (!int.TryParse(stringValue, out intValue))
                    Log.Wtf("NumericEditText.OnTextChanged", "Error trying to convert string ({0}) to int".SafeFormatTemplate(stringValue));
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

                OnIntValueChanged?.Invoke(_intValue);
            }
        }

        #endregion
    }
}
