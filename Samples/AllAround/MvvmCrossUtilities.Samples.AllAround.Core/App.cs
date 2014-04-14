using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.Localization;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Libraries.Portable.LanguageBinder;
using MvvmCrossUtilities.Samples.AllAround.Core.Services;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels;

namespace MvvmCrossUtilities.Samples.AllAround.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            InitializeResources();

            RegisterAppStart<MainViewModel>();
        }

        /// <summary>
        /// Initializes the resources.
        /// </summary>
        private void InitializeResources()
        {
            var language = "EN";

            var builder = new LocalisationProvider(language);

            Mvx.RegisterSingleton<IMvxTextProviderBuilder>(builder);

            Mvx.RegisterSingleton<IMvxTextProvider>(builder.TextProvider);

            var tlb = new TextLanguageBinder("MvvmCrossUtilities", "TextResources");
            Mvx.RegisterSingleton<ITextLanguageBinder>(tlb);
            Mvx.RegisterSingleton<IMvxLanguageBinder>(tlb);
        }
    }
}