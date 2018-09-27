using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Localization;
using MvxExtensions;
using Playground.Core.Services;
using Playground.Core.ViewModels;

namespace Playground.Core
{
    public class App : Application
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.IoCProvider.RegisterSingleton<IMvxTextProvider>(new TextProviderBuilder("EN").TextProvider);

            RegisterAppStart<MainViewModel>();
        }
    }
}
