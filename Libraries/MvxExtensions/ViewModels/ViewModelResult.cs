using MvvmCross.Base;
using MvvmCross.Localization;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxExtensions.Plugins.Notification;
using System.Threading.Tasks;

namespace MvxExtensions.ViewModels
{
    public abstract class ViewModelResult<TResult> : ViewModel, IMvxViewModelResult<TResult>
    {
        protected ViewModelResult(
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

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            if (viewFinishing && CloseCompletionSource?.Task.IsCompleted == false && !CloseCompletionSource.Task.IsFaulted)
                CloseCompletionSource?.TrySetCanceled();

            base.ViewDestroy(viewFinishing);
        }
    }
}
