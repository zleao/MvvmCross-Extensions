using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Plugin;

namespace MvxExtensions.Plugins.Storage.Platforms.iOS
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
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IStorageManager, StorageManagerIOS>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IStorageEncryptionManager, StorageEncryptionManagerIOS>();
        }
    }
}