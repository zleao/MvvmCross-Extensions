using Android.Content;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MvxExtensions.Libraries.Droid.Core.Extensions;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using System.Globalization;

namespace MvxExtensions.Libraries.Droid.Core.Components.Controls
{
    public class DecimalEditText : EditText
    {
        public delegate void DecimalValueChangedDelegate(decimal value);
        public DecimalValueChangedDelegate OnDecimalValueChanged;

        private readonly DecimalDigitsValueFilter _digitsValueFilter = new DecimalDigitsValueFilter();

        public int DecimalPlaces
        {
            get { return _digitsValueFilter.Digits; }
            set { SetDecimalPlaces(value); }
        }

        public decimal DecimalValue
        {
            get { return _decimalValue; }
            set { SetDecimalValue(value, false); }
        }
        private decimal _decimalValue;

        public DecimalEditText(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            var gravityInt = attrs.GetAttributeIntValue("http://schemas.android.com/apk/res/android", "gravity", -1);
            if (gravityInt == -1)
            {
                this.Gravity = GravityFlags.Right;
            }

            this.SetRawInputType(InputTypes.ClassNumber | InputTypes.NumberFlagDecimal);
            this.SetSingleLine(true);
            this.SetFilters(new IInputFilter[] { _digitsValueFilter });
            this.WeakSubscribeTextChanged(OnTextChanged);
        }

        private void SetDecimalPlaces(int value)
        {
            _digitsValueFilter.Digits = value;
            SetDecimalValue(DecimalValue, true);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var decimalValue = 0M;

            var stringValue = (e.Text as SpannableStringBuilder).ToString();
            if (!string.IsNullOrEmpty(stringValue))
            {
                if (!decimal.TryParse(stringValue, System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US"), out decimalValue))
                    Android.Util.Log.Wtf("DecimalEditText.OnTextChanged", "Error trying to convert string ({0}) to decimal".SafeFormatTemplate(stringValue));
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

                Text = value.ToString(format, CultureInfo.GetCultureInfo("en-US"));
            }
        }

        private void ChangeAndPropagateDecimalValue(decimal value)
        {
            if (_decimalValue != value)
            {
                _decimalValue = value;

                if (OnDecimalValueChanged != null)
                    OnDecimalValueChanged(_decimalValue);
            }
        }
    }

    internal class DecimalDigitsValueFilter : DigitsKeyListener
    {
        public int Digits
        {
            get { return _digits; }
            set
            {
                if (_digits != value)
                {
                    _digits = value;
                }
            }
        }
        private int _digits = 2;

        public DecimalDigitsValueFilter()
            : base(false, true)
        {
        }

        public override ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            var decimalSeparator = '.';//char.Parse(CultureInfo.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator);

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

            int dlen = dest.Length();

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
                        return new SpannableStringBuilder("");
                    else
                        break;  // return new SpannableStringBuilder(source, start, end);
                }
            }

            // if the dot is after the inserted part,
            // nothing can break
            return new SpannableStringBuilder(source, start, end);
        }
    }
}