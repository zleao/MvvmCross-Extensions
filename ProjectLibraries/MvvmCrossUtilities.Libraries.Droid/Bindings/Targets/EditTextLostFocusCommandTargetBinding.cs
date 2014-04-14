using System;
using System.Windows.Input;
using Android.Widget;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Droid.Target;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Targets
{
    public class EditTextLostFocusCommandTargetBinding : MvxAndroidTargetBinding
    {
        private bool _focusChangeEventSubscribed = false;

        protected ICommand LostFocusCommand
        {
            get { return _lostFocusCommand; }
        }
        private ICommand _lostFocusCommand;

        protected EditText EditText
        {
            get { return (EditText)Target; }
        }

        public EditTextLostFocusCommandTargetBinding(EditText editText)
            : base(editText)
        {
        }

        public override MvxBindingMode DefaultMode
        {
            get { return MvxBindingMode.OneWay; }
        }

        public override Type TargetType
        {
            get { return typeof(ICommand); }
        }

        protected override void SetValueImpl(object target, object value)
        {
            var command = value as ICommand;

            _lostFocusCommand = command;
            if (command == null)
            {
                if (_focusChangeEventSubscribed)
                {
                    ((EditText)target).FocusChange -= EditText_FocusChange;
                    _focusChangeEventSubscribed = false;
                }
            }
            else
            {
                if (!_focusChangeEventSubscribed)
                {
                    ((EditText)target).FocusChange += EditText_FocusChange;
                    _focusChangeEventSubscribed = true;
                }
            }
        }

        private void EditText_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus && LostFocusCommand != null)
                if (LostFocusCommand.CanExecute(null))
                    LostFocusCommand.Execute(null);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing && EditText != null && _focusChangeEventSubscribed)
                EditText.FocusChange -= EditText_FocusChange;

            base.Dispose(isDisposing);
        }
    }
}