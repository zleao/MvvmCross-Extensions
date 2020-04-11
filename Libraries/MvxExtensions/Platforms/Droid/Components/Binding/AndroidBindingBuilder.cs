using Android.Views;
using Android.Widget;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Platforms.Android.Binding;
using MvxExtensions.Platforms.Droid.Components.Controls;
using MvxExtensions.Platforms.Droid.Components.Targets;

namespace MvxExtensions.Platforms.Droid.Components.Binding
{
    /// <inheritdoc/>
    public class AndroidBindingBuilder : MvxAndroidBindingBuilder
    {
        /// <inheritdoc/>
        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            base.FillTargetFactories(registry);

            registry.RegisterFactory(new MvxCustomBindingFactory<DecimalEditText>(AndroidPropertyBinding.DecimalEditTextDecimalValue,
                decimalEditText => new DecimalEditTextDecimalValueTargetBinding(decimalEditText)));

            registry.RegisterFactory(new MvxCustomBindingFactory<EditText>(AndroidPropertyBinding.EditTextSingleLine,
                editText => new EditTextSingleLineTargetBinding(editText)));

            registry.RegisterFactory(new MvxCustomBindingFactory<NumericEditText>(AndroidPropertyBinding.NumericEditTextIntValue,
                numericEditText => new NumericEditTextIntValueTargetBinding(numericEditText)));

            registry.RegisterFactory(new MvxCustomBindingFactory<TextView>(AndroidPropertyBinding.TextViewIsValid,
                textView => new TextViewIsValidTargetBinding(textView)));

            registry.RegisterFactory(new MvxCustomBindingFactory<TextView>(AndroidPropertyBinding.TextViewMaxLength,
                textView => new TextViewMaxLengthTargetBinding(textView)));

            registry.RegisterFactory(new MvxCustomBindingFactory<ToggleButton>(AndroidPropertyBinding.ToggleButtonTextLabelOn,
                toggleButton => new ToggleButtonTextLabelOnTargetBinding(toggleButton)));

            registry.RegisterFactory(new MvxCustomBindingFactory<ToggleButton>(AndroidPropertyBinding.ToggleButtonTextLabelOff,
                toggleButton => new ToggleButtonTextLabelOffTargetBinding(toggleButton)));

            registry.RegisterFactory(new MvxCustomBindingFactory<View>(AndroidPropertyBinding.ViewIsFocused,
                view => new ViewIsFocusedTargetBinding(view)));
        }
    }
}
