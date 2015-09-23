using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace MvvmCrossUtilities.Plugins.Logger
{
    /// <summary>
    /// PluginLoader
    /// </summary>
    public class PluginLoader : IMvxPluginLoader
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly PluginLoader Instance = new PluginLoader();

        #region Implementation of IMvxPluginLoader

        /// <summary>
        /// Ensures the plugin is loaded.
        /// </summary>
        public void EnsureLoaded()
        {
            var manager = Mvx.Resolve<IMvxPluginManager>();
            manager.EnsurePlatformAdaptionLoaded<PluginLoader>();
        }

        #endregion
    }
}
