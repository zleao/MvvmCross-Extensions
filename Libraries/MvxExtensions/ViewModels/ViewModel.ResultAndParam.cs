using MvvmCross.Base;
using MvvmCross.Localization;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxExtensions.Plugins.Notification;

namespace MvxExtensions.ViewModels
{
    public abstract class ViewModel<TParameter, TResult> : ViewModelResult<TResult>, IMvxViewModel<TParameter, TResult>
    {
        protected ViewModel(
           IMvxLanguageBinder textSource,
           IMvxLanguageBinder textSourceCommon,
           IMvxJsonConverter jsonConverter,
           INotificationService notificationManager,
           IMvxLogProvider logProvider,
           IMvxNavigationService navigationService)
           : base(
               textSource,
               textSourceCommon,
               jsonConverter,
               notificationManager,
               logProvider,
               navigationService)
        {
        }

        public abstract void Prepare(TParameter parameter);
    }
}
