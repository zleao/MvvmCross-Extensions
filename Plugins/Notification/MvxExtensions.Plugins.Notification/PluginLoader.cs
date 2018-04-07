using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;

namespace MvxExtensions.Plugins.Notification
{
    /// <summary>
    /// Notification plugin loader
    /// </summary>
    public class PluginLoader : IMvxPluginLoader
    {
        /// <summary>
        /// Static instace of the plugin loader. Needed for mvx loading purposes
        /// </summary>
        public static readonly PluginLoader Instance = new PluginLoader();

        private bool _loaded;

        #region Implementation of IMvxPluginLoader

        /// <summary>
        /// Ensures that the plugin is loaded.
        /// </summary>
        public void EnsureLoaded()
        {
            if (_loaded)
            {
                return;
            }

            Mvx.RegisterSingleton<INotificationService>(new NotificationManager());
            _loaded = true;
        }

        #endregion
    }
}