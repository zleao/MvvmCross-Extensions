using System.Collections.Generic;
using System.Reflection;
using Android.Content;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Bindings.Target.Construction;
using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.Droid.Views;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Targets;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views;
using MvvmCrossUtilities.Libraries.Droid.Views;

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

        #region Overriden Methods

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

            registry.RegisterFactory(new MvxCustomBindingFactory<EditText>("SingleLine",
                                                                           editText => new EditTextSingleLineTargetBinding(editText)));

            registry.RegisterFactory(new MvxCustomBindingFactory<EditText>("LostFocusCommand",
                                                                           editText => new EditTextLostFocusCommandTargetBinding(editText)));

            registry.RegisterFactory(new MvxCustomBindingFactory<BindableViewPager>("CurrentPageIndex",
                                                                                    bindableViewPager => new BindableViewPagerCurrentPageIndexTargetBinding(bindableViewPager)));

            registry.RegisterFactory(new MvxCustomBindingFactory<ToggleButton>("TextLabelOn",
                                                                              toggleButton => new ToggleButtonTextLabelOnTargetBinding(toggleButton)));

            registry.RegisterFactory(new MvxCustomBindingFactory<ToggleButton>("TextLabelOff",
                                                                               toggleButton => new ToggleButtonTextLabelOffTargetBinding(toggleButton)));
        }

        protected override IList<Assembly> AndroidViewAssemblies
        {
            get
            {
                var assemblies = base.AndroidViewAssemblies;
                assemblies.Add(Assembly.GetExecutingAssembly());
                return assemblies;
            }
        }

        #endregion
    }
}