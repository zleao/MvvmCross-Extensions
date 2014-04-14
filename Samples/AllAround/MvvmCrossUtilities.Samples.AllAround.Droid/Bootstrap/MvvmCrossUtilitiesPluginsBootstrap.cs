using Cirrious.CrossCore.Plugins;

namespace MvvmCrossUtilities.Samples.AllAround.Droid.Bootstrap
{
    public class DevicePluginBootstrap
       : MvxPluginBootstrapAction<MvvmCrossUtilities.Plugins.Device.PluginLoader>
    {
    }

    public class LoggerPluginBootstrap
       : MvxPluginBootstrapAction<MvvmCrossUtilities.Plugins.Logger.PluginLoader>
    {
    }

    public class NotificationPluginBootstrap
       : MvxPluginBootstrapAction<MvvmCrossUtilities.Plugins.Notification.PluginLoader>
    {
    }

    public class RestPluginBootstrap
       : MvxPluginBootstrapAction<MvvmCrossUtilities.Plugins.Rest.PluginLoader>
    {
    }

    public class StoragePluginBootstrap
        : MvxPluginBootstrapAction<MvvmCrossUtilities.Plugins.Storage.PluginLoader>
    {
    }
}