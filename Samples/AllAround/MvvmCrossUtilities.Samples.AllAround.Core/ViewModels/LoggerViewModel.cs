using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.ThreadUtils;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Plugins.Logger;
using MvvmCrossUtilities.Plugins.Storage;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class LoggerViewModel : AllAroundViewModel
    {
        #region Properties

        public override string PageTitle
        {
            get
            {
                return "Logger";
            }
        }

        protected ILogger Logger
        {
            get { return _logger ?? (_logger = Mvx.Resolve<ILogger>()); }
        }
        private ILogger _logger;

        protected IStorageManager Storage
        {
            get { return _storage ?? (_storage = Mvx.Resolve<IStorageManager>()); }
        }
        private IStorageManager _storage;

        protected IMvxThreadSleep ThreadSleep
        {
            get { return _threadSleep ?? (_threadSleep = Mvx.Resolve<IMvxThreadSleep>()); }
        }
        private IMvxThreadSleep _threadSleep;

        #endregion

        #region Command

        public ICommand LogMessageCommand
        {
            get { return _logMessageCommand; }
        }
        private readonly ICommand _logMessageCommand;

        public ICommand LogExceptionCommand
        {
            get { return _logExceptionCommand; }
        }
        private readonly ICommand _logExceptionCommand;

        public ICommand LogFatalCommand
        {
            get { return _logFatalCommand; }
        }
        private readonly ICommand _logFatalCommand;

        public ICommand LogExecutionTimeCommand
        {
            get { return _logExecutionTimeCommand; }
        }
        private readonly ICommand _logExecutionTimeCommand;

        #endregion

        #region Constructor

        public LoggerViewModel()
        {
            _logMessageCommand = new MvxCommand(OnLogMessage);
            _logExceptionCommand = new MvxCommand(OnLogException);
            _logFatalCommand = new MvxCommand(OnLogFatal);
            _logExecutionTimeCommand = new MvxCommand(OnLogExecutionTime);
        }

        #endregion

        #region Methods

        private async void OnLogMessage()
        {
            await DoWorkAsync(async () =>
            {
                await Logger.LogAsync(LogTypeEnum.Info, "MvvmCrossUtilities", "Just a log message to test...");
                await ShowLogMessageAsync();
            }, "Logging...");
        }

        private async void OnLogException()
        {
            await DoWorkAsync(async () =>
            {
                await Logger.LogAsync(LogTypeEnum.Error, "MvvmCrossUtilities", new Exception("Error exception!!!"), "ErrorLog");
                await ShowLogMessageAsync();
            }, "Logging...");
        }

        private async void OnLogFatal()
        {
            await DoWorkAsync(async () =>
            {
                await Logger.LogAsync(LogTypeEnum.Fatal, "MvvmCrossUtilities", new Exception("Fatal exception!!!"), "FatalLog");
                await ShowLogMessageAsync();
            }, "Logging...");
        }

        private async void OnLogExecutionTime()
        {
            await DoWorkAsync(async () =>
            {
                await Logger.LogMethodExecutionTimeAsync(() => ThreadSleep.Sleep(new TimeSpan(0, 0, 1)), "OnLogExecutionTime");
                await ShowLogMessageAsync();
            }, "Logging...");
        }
        private Task ShowLogMessageAsync()
        {
            return PublishInfoNotificationAsync("Log written" + Environment.NewLine + "Check folder: " + Storage.NativePath(StorageLocation.ExternalPublic, Logger.LogBasePath));
        }

        #endregion
    }
}
