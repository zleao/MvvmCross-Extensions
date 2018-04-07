using MvvmCross.Core.ViewModels;
using MvvmCross.Localization;
using MvvmCross.Platform.Platform;
using MvxExtensions.Libraries.Portable.Core.Services.Logger;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Plugins.Notification.Messages;
using MvxExtensions.Plugins.Notification.Messages.TwoWay.Question;
using MvxExtensions.Samples.AllAround.Core.ViewModels.Base;
using System.Windows.Input;

namespace MvxExtensions.Samples.AllAround.Core.ViewModels
{
    public class NotificationsViewModel : SimpleMenuBaseViewModel
    {
        #region Fields

        private string _mainViewModelContext = null;

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

        public NotificationsViewModel(IMvxLanguageBinder textSource,
                                      IMvxJsonConverter jsonConverter,
                                      INotificationService notificationManager,
                                      ILoggerManager loggerManager)
            : base(textSource, jsonConverter, notificationManager, loggerManager)
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
            await NotificationManager.PublishErrorNotificationAsync("Error notification", NotificationModeEnum.MessageBox);
        }

        private async void OnQuestionNotification()
        {
            var answer = await NotificationManager.PublishGenericQuestionNotificationAsync("Do you feel lucky?", NotificationTwoWayAnswersGroupEnum.YesNo);
            if (answer.Answer == NotificationTwoWayAnswersEnum.Yes)
                await NotificationManager.PublishSuccessNotificationAsync("That's the spirit!");
            else
                await NotificationManager.PublishWarningNotificationAsync("You can do it!");
        }

        private async void OnDelayedNotification()
        {
            await NotificationManager.DelayedPublishSuccessNotificationAsync("Delayed notification sent by NotificationsViewModel", NotificationModeEnum.MessageBox, _mainViewModelContext);
            await NotificationManager.PublishInfoNotificationAsync("Go back to the Main to be able to see the delayed notification work his/her magic", NotificationModeEnum.MessageBox);
        }

        #endregion
    }
}
