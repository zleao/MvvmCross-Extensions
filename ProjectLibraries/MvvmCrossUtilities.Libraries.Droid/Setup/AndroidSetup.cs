using Android.Content;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Bindings.Target.Construction;
using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.Views;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Targets;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views;
using MvvmCrossUtilities.Libraries.Droid.Views;
using System.Collections.Generic;
using System.Reflection;

namespace MvvmCrossUtilities.Libraries.Droid.Setup
{
    public abstract class AndroidSetup : MvxAndroidSetup
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAndroidBindingSetup"/> class.
        /// </summary>
        /// <param name="aplicationContext">The aplication context.</param>
        public AndroidSetup(Context aplicationContext)
            : base(aplicationContext)
        {
        }
        
        #endregion

        #region Methods

        protected override IMvxAndroidViewsContainer CreateViewsContainer(Context applicationContext)
        {
            return new ViewsContainer(applicationContext);
        }
        
        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            base.FillTargetFactories(registry);

            registry.RegisterFactory(new MvxCustomBindingFactory<View>("IsFocused",
                                                                       view => new ViewIsFocusedTargetBinding(view)));

            registry.RegisterFactory(new MvxCustomBindingFactory<TextView>("IsValid",
                                                                           textView => new TextViewIsValidTargetBinding(textView)));
            registry.RegisterFactory(new MvxCustomBindingFactory<TextView>("TextLabel",
                                                                           textView => new TextViewTextLabelTargetBinding(textView)));
            registry.RegisterFactory(new MvxCustomBindingFactory<TextView>("MaxLength",
                                                                           textView => new TextViewMaxLengthTargetBinding(textView)));

            registry.RegisterFactory(new MvxCustomBindingFactory<EditText>("SingleLine",
                                                                           editText => new EditTextSingleLineTargetBinding(editText)));
            registry.RegisterFactory(new MvxCustomBindingFactory<EditText>("LostFocusCommand",
                                                                           editText => new EditTextLostFocusCommandTargetBinding(editText)));

            registry.RegisterFactory(new MvxCustomBindingFactory<BindableViewPager>("CurrentPageIndex",
                                                                                    bindableViewPager => new BindableViewPagerCurrentPageIndexTargetBinding(bindableViewPager)));

            registry.RegisterFactory(new MvxCustomBindingFactory<BindableViewPager>("CurrentPage", 
                                                                                    bindableViewPager => new BindableViewPagerCurrentPageTargetBinding(bindableViewPager)));

            registry.RegisterFactory(new MvxCustomBindingFactory<ToggleButton>("TextLabelOn",
                                                                               toggleButton => new ToggleButtonTextLabelOnTargetBinding(toggleButton)));

            registry.RegisterFactory(new MvxCustomBindingFactory<ToggleButton>("TextLabelOff",
                                                                               toggleButton => new ToggleButtonTextLabelOffTargetBinding(toggleButton)));

            registry.RegisterFactory(new MvxCustomBindingFactory<DecimalEditText>("DecimalValue", decimalEditText => new DecimalEditTextDecimalValueTargetBinding(decimalEditText)));

            registry.RegisterFactory(new MvxCustomBindingFactory<NumericEditText>("IntValue", numericEditText => new NumericEditTextIntValueTargetBinding(numericEditText)));
        }

        protected override IList<Assembly> AndroidViewAssemblies
        {
            get
            {
                var assemblies = base.AndroidViewAssemblies;
                assemblies.Add(Assembly.GetExecutingAssembly());
                assemblies.Add(typeof(PagerTabStrip).Assembly);
                return assemblies;
            }
        }

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            return new AndroidViewPresenter();
        }

        protected override IMvxViewDispatcher CreateViewDispatcher()
        {
            var presenter = CreateViewPresenter() as IAndroidViewPresenter;
            return new AndroidViewDispatcher(presenter);
        }

        #endregion
    }
}