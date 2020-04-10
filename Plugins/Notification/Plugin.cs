using MvvmCross;
using MvvmCross.Plugin;

namespace MvxExtensions.Plugins.Notification
{
    /// <summary>
    /// Plugin registration class
    /// </summary>
    [MvxPlugin]
    [Preserve(AllMembers = true)]
    public class Plugin : IMvxPlugin
    {
        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            Mvx.IoCProvider.RegisterSingleton<INotificationService>(new NotificationManager());
        }
    }
}