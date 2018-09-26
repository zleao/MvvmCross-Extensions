using System.Collections.Generic;
using System.Reflection;
using MvvmCross;
using MvvmCross.Platforms.Android.Presenters;
using MvxExtensions.Platforms.Android.Presenters;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.ViewModels;

namespace MvxExtensions.Platforms.Android.Setup
{
    public abstract class AndroidAppCompatSetup : MvxAppCompatSetup
    {
        protected override IEnumerable<Assembly> AndroidViewAssemblies =>
            new List<Assembly>(base.AndroidViewAssemblies)
            {
                typeof(MvxRecyclerView).Assembly
            };

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            return new AndroidAppCompatViewPresenter(AndroidViewAssemblies);
        }
    }

    public class AndroidAppCompatSetup<TApplication> : AndroidAppCompatSetup
        where TApplication : IMvxApplication, new()
    {
        protected override IMvxApplication CreateApp() => Mvx.IocConstruct<TApplication>();

        public override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            return new[] { typeof(TApplication).GetTypeInfo().Assembly };
        }
    }
}
