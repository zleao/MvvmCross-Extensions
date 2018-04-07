using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;

namespace MvxExtensions.Plugins.Storage
{
    /// <summary>
    /// PluginLoader
    /// </summary>
    public class PluginLoader: IMvxPluginLoader
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly PluginLoader Instance = new PluginLoader();

        /// <summary>
        /// Ensures the plugin is loaded.
        /// </summary>
        public void EnsureLoaded()
        {
            var manager = Mvx.Resolve<IMvxPluginManager>();
            manager.EnsurePlatformAdaptionLoaded<PluginLoader>();
        }
    }
}
