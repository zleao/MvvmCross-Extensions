using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;

namespace MvxExtensions.Plugins.Storage.Wpf
{
    /// <summary>
    /// Base class for plugin initialization
    /// </summary>
    /// <seealso cref="MvvmCross.Platform.Plugins.IMvxPlugin" />
    public class Plugin : IMvxPlugin
    {
        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton<IStorageManager, StorageManager>();
        }
    }
}
