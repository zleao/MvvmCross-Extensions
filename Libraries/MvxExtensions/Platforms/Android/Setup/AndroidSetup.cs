using System.Collections.Generic;
using System.Reflection;
using MvvmCross;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.ViewModels;
using MvxExtensions.Platforms.Android.Presenters;

namespace MvxExtensions.Platforms.Android.Setup
{
    public abstract class AndroidSetup : MvxAndroidSetup
    {
        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            return new AndroidViewPresenter(AndroidViewAssemblies);
        }
    }

    public class AndroidSetup<TApplication> : AndroidSetup
        where TApplication : IMvxApplication, new()
    {
        protected override IMvxApplication CreateApp() => Mvx.IocConstruct<TApplication>();

        public override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            return new[] { typeof(TApplication).GetTypeInfo().Assembly };
        }
    }
}
