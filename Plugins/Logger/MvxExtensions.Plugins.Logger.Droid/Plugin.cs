using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;

namespace MvxExtensions.Plugins.Logger.Droid
{
    /// <summary>
    /// Logger plugin loader
    /// </summary>
    public class Plugin : IMvxPlugin
    {
        /// <summary>
        /// Static instace of the plugin loader. Needed for mvx loading purposes
        /// </summary>
        public void Load()
        {
            Mvx.ConstructAndRegisterSingleton<ILogger, Logger>();
        }
    }
}