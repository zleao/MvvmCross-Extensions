using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;
using System.Windows.Input;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class NotificationsViewModel : AllAroundViewModel
    {
        #region Fields

        private string _mainViewModelContext = null;

        #endregion

        #region Properties

        public override string PageTitle
        {
            get { return "Notifications"; }
        }

        #endregion

        #region Commands

        public ICommand ErrorNotificationCommand
        {
            get { return _errorNotificationCommand; }
        }
        private readonly ICommand _errorNotificationCommand;

        public ICommand QuestionNotificationCommand
        {
            get { return _questionNotificationCommand; }
        }
        private readonly ICommand _questionNotificationCommand;

        public ICommand DelayedNotificationCommand
        {
            get { return _delayedNotificationCommand; }
        }
        private readonly ICommand _delayedNotificationCommand;

        #endregion

        #region Constructor

        public NotificationsViewModel()
        {
            _errorNotificationCommand = new MvxCommand(OnErrorNotification);
            _questionNotificationCommand = new MvxCommand(OnQuestionNotification);
            _delayedNotificationCommand = new MvxCommand(OnDelayedNotification);
        }

        #endregion

        #region Methods

        public void Init(string mainViewModelContext)
        {
            _mainViewModelContext = mainViewModelContext;
        }

        private async void OnErrorNotification()
        {
            await PublishErrorNotificationAsync("Error notification", NotificationModeEnum.MessageBox);
        }

        private async void OnQuestionNotification()
        {
            var answer = await PublishGenericQuestionNotificationAsync("Do you feel lucky?", NotificationTwoWayAnswersGroupEnum.YesNo);
            if (answer.Answer == NotificationTwoWayAnswersEnum.Yes)
                await PublishSuccessNotificationAsync("That's the spirit!");
            else
                await PublishWarningNotificationAsync("You can do it!");
        }

        private async void OnDelayedNotification()
        {
            await DelayedPublishSuccessNotificationAsync("Delayed notification sent by NotificationsViewModel", NotificationModeEnum.MessageBox, _mainViewModelContext);
            await PublishInfoNotificationAsync("Go back to the Main to be able to see the delayed notification work his/her magic", NotificationModeEnum.MessageBox);
        }

        #endregion
    }
}
