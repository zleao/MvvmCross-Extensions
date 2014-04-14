using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace MvvmCrossUtilities.Plugins.Notification
{
    public class PluginLoader : IMvxPluginLoader
    {
        public static readonly PluginLoader Instance = new PluginLoader();

        private bool _loaded;

        #region Implementation of IMvxPluginLoader

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