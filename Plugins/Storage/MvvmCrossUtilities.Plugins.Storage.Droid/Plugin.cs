using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace MvvmCrossUtilities.Plugins.Storage.Droid
{
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton<IStorageManager, StorageManager>();
        }
    }
}