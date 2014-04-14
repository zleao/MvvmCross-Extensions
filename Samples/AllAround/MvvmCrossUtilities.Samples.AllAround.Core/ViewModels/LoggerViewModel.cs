using System;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.ThreadUtils;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Plugins.Logger;
using MvvmCrossUtilities.Plugins.Storage;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;

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

        private void OnLogMessage()
        {
            Logger.Log(LogLevel.Info, "MvvmCrossUtilities", "Just a log message to test...");
            ShowLogMessage();
        }

        private void OnLogException()
        {
            Logger.Log(LogLevel.Error, "MvvmCrossUtilities", new Exception("Error exception!!!"), "ErrorLog");
            ShowLogMessage();
        }

        private void OnLogFatal()
        {
            Logger.Log(LogLevel.Fatal, "MvvmCrossUtilities", new Exception("Fatal exception!!!"), "FatalLog");
            ShowLogMessage();
        }

        private void ShowLogMessage()
        {
            PublishInfoNotification("Log written" + Environment.NewLine + "Check folder: " + Storage.NativePath(StorageLocation.ExternalPublic, Logger.LogBasePath));
        }

        private void OnLogExecutionTime()
        {
            Logger.LogExecutionTime(() => ThreadSleep.Sleep(new TimeSpan(0, 0, 1)), "OnLogExecutionTime");
            ShowLogMessage();
        }

        #endregion
    }
}
