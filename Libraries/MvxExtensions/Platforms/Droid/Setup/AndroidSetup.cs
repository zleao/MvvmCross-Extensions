using System.Collections.Generic;
using System.Reflection;
using MvvmCross;
using MvvmCross.Binding;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.ViewModels;
using MvxExtensions.Platforms.Droid.Presenters;
using MvxExtensions.Platforms.Droid.Components.Binding;

namespace MvxExtensions.Platforms.Droid.Setup
{
    public abstract class AndroidSetup : MvxAndroidSetup
    {
        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            return new AndroidViewPresenter(AndroidViewAssemblies);
        }

        protected override MvxBindingBuilder CreateBindingBuilder()
        {
            return new AndroidBindingBuilder();
        }
    }

    public class AndroidSetup<TApplication> : AndroidSetup
        where TApplication : class, IMvxApplication, new()
    {
        protected override IMvxApplication CreateApp() => Mvx.IoCProvider.IoCConstruct<TApplication>();

        public override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            return new[] { typeof(TApplication).GetTypeInfo().Assembly };
        }
    }
}
