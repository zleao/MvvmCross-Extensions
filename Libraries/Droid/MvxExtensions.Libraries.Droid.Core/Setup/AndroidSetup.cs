using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Core.Views;
using MvvmCross.Droid.Platform;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;
using MvxExtensions.Libraries.Droid.Core.Components.Controls;
using MvxExtensions.Libraries.Droid.Core.Components.Targets;
using MvxExtensions.Libraries.Droid.Core.Support.V4.Setup;
using MvxExtensions.Libraries.Droid.Core.Views;
using MvxExtensions.Libraries.Droid.Core.Views.MvxOverrides;
using System.Collections.Generic;
using System.Reflection;

namespace MvxExtensions.Libraries.Droid.Core.Setup
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

        protected override void InitializePlatformServices()
        {
            base.InitializePlatformServices();

            var viewModelNullCache = new NullViewModelCache();
            Mvx.RegisterSingleton<IMvxSingleViewModelCache>(viewModelNullCache);
        }

        protected override IMvxAndroidViewsContainer CreateViewsContainer(Context applicationContext)
        {
            return new ViewsContainer(applicationContext);
        }

        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            base.FillTargetFactories(registry);

            MvxAppCompatSetupHelper.FillTargetFactories(registry);

            SupportV4SetupHelper.FillTargetFactories(registry);

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

            
            registry.RegisterFactory(new MvxCustomBindingFactory<ToggleButton>("TextLabelOn",
                                                                               toggleButton => new ToggleButtonTextLabelOnTargetBinding(toggleButton)));

            registry.RegisterFactory(new MvxCustomBindingFactory<ToggleButton>("TextLabelOff",
                                                                               toggleButton => new ToggleButtonTextLabelOffTargetBinding(toggleButton)));

            registry.RegisterFactory(new MvxCustomBindingFactory<DecimalEditText>("DecimalValue", decimalEditText => new DecimalEditTextDecimalValueTargetBinding(decimalEditText)));

            registry.RegisterFactory(new MvxCustomBindingFactory<NumericEditText>("IntValue", numericEditText => new NumericEditTextIntValueTargetBinding(numericEditText)));
        }

        protected override void FillBindingNames(IMvxBindingNameRegistry registry)
        {
            base.FillBindingNames(registry);

            MvxAppCompatSetupHelper.FillDefaultBindingNames(registry);

            registry.AddOrOverwrite(typeof(ImageStreamView), "ImageStream");
            registry.AddOrOverwrite(typeof(ImageUrlView), "ImageUrl");
            registry.AddOrOverwrite(typeof(ExpandedHeightGridView), "ItemsSource");
        }

        protected override IEnumerable<Assembly> AndroidViewAssemblies
        {
            get
            {
                var assemblies = base.AndroidViewAssemblies as IList<Assembly>;
                assemblies.Add(Assembly.GetExecutingAssembly());
                assemblies.Add(typeof(PagerTabStrip).Assembly);
                assemblies.Add(typeof(TabLayout).Assembly);
                assemblies.Add(typeof(Android.Support.V7.Widget.Toolbar).Assembly);
                assemblies.Add(typeof(Android.Support.V4.Widget.DrawerLayout).Assembly);
                assemblies.Add(typeof(MvvmCross.Droid.Support.V7.RecyclerView.MvxRecyclerView).Assembly);

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