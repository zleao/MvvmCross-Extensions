using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Localization;
using MvxExtensions;
using Playground.Forms.Core.Services;
using Playground.Forms.Core.ViewModels;

namespace Playground.Forms.Core
{
    public class App : Application
    {
        public override void Initialize()
        {
            base.Initialize();

            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.IoCProvider.RegisterSingleton<IMvxTextProvider>(new TextProviderBuilder("EN").TextProvider);

            RegisterAppStart<MainViewModel>();
        }
    }
}
