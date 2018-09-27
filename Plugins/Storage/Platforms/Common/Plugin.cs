using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Plugin;

#if ANDROID
namespace MvxExtensions.Plugins.Storage.Droid
#elif XAML
namespace MvxExtensions.Plugins.Storage.Wpf
#else
namespace MvxExtensions.Plugins.Storage
#endif
{
    [MvxPlugin]
    [Preserve(AllMembers = true)]
    public class Plugin : IMvxPlugin
    {
        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IStorageManager, StorageManager>();
        }
    }
}