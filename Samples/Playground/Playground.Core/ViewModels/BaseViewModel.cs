using MvvmCross.Localization;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxExtensions.Plugins.Notification;

namespace Playground.Core.ViewModels
{
    public abstract class BaseViewModel : MvxViewModel
    {
        #region Properties

        public IMvxLanguageBinder TextSourceCommon => new MvxLanguageBinder("Playground.Core", "Common");
        public IMvxLanguageBinder TextSource => new MvxLanguageBinder("Playground.Core", GetType().Name);
        protected IMvxNavigationService NavigationService {get;}
        protected IMvxLogProvider LogProvider {get;}
        public INotificationService NotificationManager { get; }

        #endregion

        #region Constructor

        public BaseViewModel(IMvxNavigationService navigationService, IMvxLogProvider logProvider, INotificationService notificationManager)
        {
            NavigationService = navigationService;
            LogProvider = logProvider;
            NotificationManager = notificationManager;
        }

        #endregion
    }
}
