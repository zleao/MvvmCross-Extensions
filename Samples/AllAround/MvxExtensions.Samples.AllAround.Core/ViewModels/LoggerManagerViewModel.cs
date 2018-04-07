using MvvmCross.Core.ViewModels;
using MvvmCross.Localization;
using MvvmCross.Platform.Platform;
using MvxExtensions.Libraries.Portable.Core.Services.Logger;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Samples.AllAround.Core.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvxExtensions.Samples.AllAround.Core.ViewModels
{
    public class LoggerManagerViewModel : SimpleMenuBaseViewModel
    {
        #region Command

        public ICommand LogMessageCommand => _logMessageCommand;
        private readonly ICommand _logMessageCommand;

        public ICommand LogExceptionCommand => _logExceptionCommand;
        private readonly ICommand _logExceptionCommand;

        public ICommand LogFatalCommand => _logFatalCommand;
        private readonly ICommand _logFatalCommand;

        #endregion

        #region Constructor

        public LoggerManagerViewModel(IMvxLanguageBinder textSource,
                                      IMvxJsonConverter jsonConverter,
                                      INotificationService notificationManager,
                                      ILoggerManager loggerManager)
            : base(textSource, jsonConverter, notificationManager, loggerManager)
        {
            _logMessageCommand = new MvxCommand(OnLogMessage);
            _logExceptionCommand = new MvxCommand(OnLogException);
            _logFatalCommand = new MvxCommand(OnLogFatal);
        }

        #endregion

        #region Methods

        private async void OnLogMessage()
        {
            await DoWorkAsync(async () =>
            {
                await LoggerManager.LogInfoAsync("Just a log message to test...", "InfoLog");
                await ShowLogMessageAsync();
            }, "Logging...");
        }

        private async void OnLogException()
        {
            await DoWorkAsync(async () =>
            {
                await LoggerManager.LogErrorAsync("There was an error", new Exception("Error exception!!!"), "ErrorLog");
                await ShowLogMessageAsync();
            }, "Logging...");
        }

        private async void OnLogFatal()
        {
            await DoWorkAsync(async () =>
            {
                await LoggerManager.LogFatalAsync("There was a fatal error", new Exception("Fatal exception!!!"), "FatalLog");
                await ShowLogMessageAsync();
            }, "Logging...");
        }

       
        private Task ShowLogMessageAsync()
        {
            return NotificationManager.PublishInfoNotificationAsync("Log written" + Environment.NewLine + "Check folder: " + LoggerManager.LogFolderFullPath);
        }

        #endregion
    }
}
