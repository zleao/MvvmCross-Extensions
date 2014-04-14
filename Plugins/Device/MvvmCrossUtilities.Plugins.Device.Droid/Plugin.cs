using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace MvvmCrossUtilities.Plugins.Device.Droid
{
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.ConstructAndRegisterSingleton<IDevice, Device>();
        }
    }
}