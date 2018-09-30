using System.Globalization;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;
using MvxExtensions.Extensions;
using MvxExtensions.Platforms.Droid.Extensions;
using Context = Android.Content.Context;

namespace MvxExtensions.Platforms.Droid.Components.Controls
{
    [Register("mvxextensions.platforms.droid.components.controls.DecimalEditText")]
    public class DecimalEditText : EditText
    {
        public delegate void DecimalValueChangedDelegate(decimal value);
        public DecimalValueChangedDelegate OnDecimalValueChanged;

        #region Properties

        protected DecimalDigitsValueFilter DigitsValueFilter => new DecimalDigitsValueFilter();

        public int DecimalPlaces
        {
            get => DigitsValueFilter.Digits;
            set => SetDecimalPlaces(value);
        }

        public decimal DecimalValue
        {
            get => _decimalValue;
            set => SetDecimalValue(value, false);
        }
        private decimal _decimalValue;

        #endregion

        #region Constructor

        public DecimalEditText(Context context)
            : base(context)
        {
            Initialize(null);
        }

        public DecimalEditText(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize(attrs);
        }

        public DecimalEditText(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Initialize(attrs);
        }

        public DecimalEditText(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize(attrs);
        }

        #endregion

        private void Initialize(IAttributeSet attrs)
        {
            var gravityInt = attrs.GetAttributeIntValue("http://schemas.android.com/apk/res/android", "gravity", -1);
            if (gravityInt == -1)
            {
                Gravity = GravityFlags.Right;
            }

            SetRawInputType(InputTypes.ClassNumber | InputTypes.NumberFlagDecimal);
            SetSingleLine(true);
            SetFilters(new IInputFilter[] { DigitsValueFilter });
            this.WeakSubscribeTextChanged(OnTextChanged);
        }

        private void SetDecimalPlaces(int value)
        {
            DigitsValueFilter.Digits = value;
            SetDecimalValue(DecimalValue, true);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var decimalValue = 0M;

            var stringValue = (e.Text as SpannableStringBuilder)?.ToString();
            if (!string.IsNullOrEmpty(stringValue))
            {
                if (!decimal.TryParse(stringValue, NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US"), out decimalValue))
                    Log.Wtf("DecimalEditText.OnTextChanged", "Error trying to convert string ({0}) to decimal".SafeFormatTemplate(stringValue));
            }

            ChangeAndPropagateDecimalValue(decimalValue);
        }

        private void SetDecimalValue(decimal value, bool forceUpdate)
        {
            if (forceUpdate || _decimalValue != value || Text.IsNullOrEmpty())
            {
                _decimalValue = value;

                var format = "0.";
                for (int i = 0; i < DecimalPlaces; i++)
                    format += "#";

                Text = value.ToString(format, CultureInfo.CurrentUICulture);
            }
        }

        private void ChangeAndPropagateDecimalValue(decimal value)
        {
            if (_decimalValue != value)
            {
                _decimalValue = value;

                OnDecimalValueChanged?.Invoke(_decimalValue);
            }
        }
    }

    public class DecimalDigitsValueFilter : DigitsKeyListener
    {
        public int Digits { get; set; } = 2;

        public DecimalDigitsValueFilter()
            : base(Locale.Default, false, true)
        {
        }

        public override ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            var decimalSeparator = '.';

            ICharSequence outValue = base.FilterFormatted(source, start, end, dest, dstart, dend);

            // if changed, replace the source
            if (outValue != null)
            {
                source = outValue;
                start = 0;
                end = outValue.Length();
            }

            int len = end - start;

            // if deleting, source is empty
            // and deleting can't break anything
            if (len == 0)
            {
                return source;
            }

            var dlen = dest.Length();

            // Find the position of the decimal .
            for (int i = 0; i < dstart; i++)
            {
                if (dest.CharAt(i) == decimalSeparator)
                {
                    // being here means, that a number has
                    // been inserted after the dot
                    // check if the amount of digits is right
                    return (dlen - (i + 1) + len > Digits) ?
                        new SpannableStringBuilder("") :
                        new SpannableStringBuilder(source, start, end);
                }
            }

            for (int i = start; i < end; ++i)
            {
                if (source.CharAt(i) == decimalSeparator)
                {
                    // being here means, dot has been inserted
                    // check if the amount of digits is right
                    if ((dlen - dend) + (end - (i + 1)) > Digits)
                    {
                        return new SpannableStringBuilder("");
                    }

                    break;
                }
            }

            // if the dot is after the inserted part,
            // nothing can break
            return new SpannableStringBuilder(source, start, end);
        }
    }
}
