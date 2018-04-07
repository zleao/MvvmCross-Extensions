using MvvmCross.Localization;
using MvvmCross.Platform.Platform;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using MvxExtensions.Libraries.Portable.Core.Services.Logger;
using MvxExtensions.Libraries.Portable.Core.ViewModels.SimpleMenu;
using MvxExtensions.Plugins.Notification;

namespace MvxExtensions.Samples.AllAround.Core.ViewModels.Base
{
    public abstract class SimpleMenuBaseViewModel : SimpleMenuViewModel
    {
        public override ILoggerManager LoggerManager
        {
            get { return _loggerManager; }
        }
        public readonly ILoggerManager _loggerManager;

        protected SimpleMenuBaseViewModel(IMvxLanguageBinder textSource,
                                     IMvxJsonConverter jsonConverter,
                                     INotificationService notificationManager,
                                     ILoggerManager loggerManager)
            : base(textSource, jsonConverter, notificationManager)
        {
            _loggerManager = loggerManager.ThrowIfIoComponentIsNull(nameof(loggerManager));
        }
    }
}
