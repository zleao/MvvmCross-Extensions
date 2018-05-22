using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Plugins.Notification.Messages;
using MvxExtensions.Plugins.Notification.Messages.TwoWay.Question;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Playground.Core.ViewModels
{
    public class NotificationsViewModel : BaseViewModel
    {
        #region Fields

        private string _mainViewModelContext = null;

        #endregion

        #region Commands

        public ICommand ErrorNotificationCommand { get; }
        public ICommand QuestionNotificationCommand { get; }
        public ICommand DelayedNotificationCommand { get; }

        #endregion

        #region Constructor

        public NotificationsViewModel(IMvxNavigationService navigationService, IMvxLogProvider logProvider, INotificationService notificationManager)
            : base(navigationService, logProvider, notificationManager)
        {
            ErrorNotificationCommand = new MvxAsyncCommand(OnErrorNotificationAsync);
            QuestionNotificationCommand = new MvxAsyncCommand(OnQuestionNotificationAsync);
            DelayedNotificationCommand = new MvxAsyncCommand(OnDelayedNotificationAsync);
        }

        #endregion

        #region Methods

        public void Init(string mainViewModelContext)
        {
            _mainViewModelContext = mainViewModelContext;
        }

        private Task OnErrorNotificationAsync()
        {
            return NotificationManager.PublishErrorNotificationAsync("Error notification", NotificationModeEnum.MessageBox);
        }

        private async Task OnQuestionNotificationAsync()
        {
            var answer = await NotificationManager.PublishGenericQuestionNotificationAsync("Do you feel lucky?", NotificationTwoWayAnswersGroupEnum.YesNo);
            if (answer.Answer == NotificationTwoWayAnswersEnum.Yes)
                await NotificationManager.PublishSuccessNotificationAsync("That's the spirit!");
            else
                await NotificationManager.PublishWarningNotificationAsync("You can do it!");
        }

        private async Task OnDelayedNotificationAsync()
        {
            await NotificationManager.DelayedPublishSuccessNotificationAsync("Delayed notification sent by NotificationsViewModel", NotificationModeEnum.MessageBox, _mainViewModelContext);
            await NotificationManager.PublishInfoNotificationAsync("Go back to the Main to be able to see the delayed notification work his/her magic", NotificationModeEnum.MessageBox);
        }

        #endregion
    }
}
