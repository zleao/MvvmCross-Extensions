using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class CustomEditTextViewModel : AllAroundViewModel
    {
        #region Properties

        public override string PageTitle
        {
            get { return "Custom EditText"; }
        }

        public int MaxDecimalPlaces
        {
            get { return _maxDecimalPlaces; }
            set
            {
                if (_maxDecimalPlaces != value)
                {
                    _maxDecimalPlaces = value;
                    RaisePropertyChanged(() => MaxDecimalPlaces);
                }
            }
        }
        private int _maxDecimalPlaces = 2;

        public decimal DecimalValue
        {
            get { return _decimalValue; }
            set
            {
                if (_decimalValue != value)
                {
                    _decimalValue = value;
                    RaisePropertyChanged(() => DecimalValue);
                }
            }
        }
        private decimal _decimalValue = 0M;

        public int IntValue
        {
            get { return _intValue; }
            set
            {
                if (_intValue != value)
                {
                    _intValue = value;
                    RaisePropertyChanged(() => IntValue);
                }
            }
        }
        private int _intValue;

        #endregion

        #region Constructor

        public CustomEditTextViewModel()
        {
        }

        #endregion
    }
}
