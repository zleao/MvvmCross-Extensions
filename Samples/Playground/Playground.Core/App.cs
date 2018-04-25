using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Localization;
using MvvmCross.ViewModels;
using Playground.Core.Services;
using Playground.Core.ViewModels;

namespace Playground.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.RegisterSingleton<IMvxTextProvider>(new TextProviderBuilder("EN").TextProvider);

            RegisterAppStart<MainViewModel>();
        }

        /// <summary>
        /// Do any UI bound startup actions here
        /// </summary>
        /// <param name="hint"></param>
        public override void Startup(object hint)
        {
            base.Startup(hint);
        }

        /// <summary>
        /// If the application is restarted (eg primary activity on Android 
        /// can be restarted) this method will be called before Startup
        /// is called again
        /// </summary>
        public override void Reset()
        {
            base.Reset();
        }
    }
}
