using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;

namespace MvxExtensions.Plugins.Logger.Wpf
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
            Mvx.ConstructAndRegisterSingleton<ILogger, Logger>();
        }
    }
}
