using MvvmCross.Core.ViewModels;
using MvvmCross.Localization;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using MvvmCross.Plugins.JsonLocalization;
using MvxExtensions.Libraries.Portable.Core.Services.LanguageBinder;
using MvxExtensions.Samples.AllAround.Core.Services;
using MvxExtensions.Samples.AllAround.Core.ViewModels;

namespace MvxExtensions.Samples.AllAround.Core
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

            var builder = new LocalizationProvider(language);

            Mvx.RegisterSingleton<IMvxTextProviderBuilder>(builder);

            Mvx.RegisterSingleton<IMvxTextProvider>(builder.TextProvider);

            var tlb = new TextLanguageBinder("MvxExtensions", "TextResources");
            Mvx.RegisterSingleton<ITextLanguageBinder>(tlb);
            Mvx.RegisterSingleton<IMvxLanguageBinder>(tlb);
        }
    }
}