using MvvmCross.Core;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.ViewModels;
using MvxExtensions.Platforms.iOS.Setup;

namespace MvxExtensions.Platforms.iOS
{
    public abstract class ApplicationDelegate<TMvxIosSetup, TApplication> : MvxApplicationDelegate
       where TMvxIosSetup : iOSSetup<TApplication>, new()
       where TApplication : class, IMvxApplication, new()
    {
        protected override void RegisterSetup()
        {
            this.RegisterSetupType<TMvxIosSetup>();
        }
    }
}
