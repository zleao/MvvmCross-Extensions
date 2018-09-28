using System.Collections.Generic;
using System.Reflection;
using MvvmCross;
using MvvmCross.Binding;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.ViewModels;
using MvxExtensions.Platforms.Droid.Components.Binding;
using MvxExtensions.Platforms.Droid.Support.V7.Presenters;

namespace MvxExtensions.Platforms.Droid.Support.V7.Setup
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

        protected override MvxBindingBuilder CreateBindingBuilder()
        {
            return new AndroidBindingBuilder();
        }
    }

    public class AndroidAppCompatSetup<TApplication> : AndroidAppCompatSetup
        where TApplication : class, IMvxApplication, new()
    {
        protected override IMvxApplication CreateApp() => Mvx.IoCProvider.IoCConstruct<TApplication>();

        public override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            return new[] { typeof(TApplication).GetTypeInfo().Assembly };
        }
    }
}
