using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.Plugin.Json;
using MvvmCross.ViewModels;
using MvxExtensions.Platforms.iOS.Presenters;
using System.Collections.Generic;
using System.Reflection;

namespace MvxExtensions.Platforms.iOS.Setup
{
    public abstract class iOSSetup : MvxIosSetup
    {
       protected override void InitializeFirstChance()
        {
            //TODO: workaround to register the IMvxJsonConverter. NEed to check why the base registration is not working
            Mvx.IoCProvider.RegisterSingleton<IMvxJsonConverter>(new MvxJsonConverter());

            base.InitializeFirstChance();
        }

        protected override IMvxIosViewPresenter CreateViewPresenter()
        {
            return new iOSViewPresenter(ApplicationDelegate, Window);
        }
    }

    public class iOSSetup<TApplication> : iOSSetup
        where TApplication : class, IMvxApplication, new()
    {
        protected override IMvxApplication CreateApp() => Mvx.IoCProvider.IoCConstruct<TApplication>();

        public override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            return new[] { typeof(TApplication).GetTypeInfo().Assembly };
        }
    }
}
