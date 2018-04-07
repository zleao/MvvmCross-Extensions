using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using MvxExtensions.Plugins.Logger;
using MvxExtensions.Plugins.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MvxExtensions.Libraries.Portable.Core.Services.Logger
{
    /// <summary>
    /// The logger manager base class implementation
    /// </summary>
    public class LoggerManager : ILoggerManager
    {
        #region Properties

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected virtual ILogger Logger
        {
            get
            {
                if (IsLoggerRegistered)
                    return Mvx.Resolve<ILogger>();
                else
                    return _logger ?? (_logger = new DummyLogger());
            }
        }
        private ILogger _logger;

        /// <summary>
        /// Gets the logger manager configuration.
        /// </summary>
        protected LoggerConfiguration Configuration
        {
            get { return _configuration; }
        }
        private readonly LoggerConfiguration _configuration;

        /// <summary>
        /// Indicates if the logger is registered.
        /// </summary>
        protected virtual bool IsLoggerRegistered
        {
            get { return Mvx.CanResolve<ILogger>(); }
        }

        /// <summary>
        /// Gets the log folder relative path.
        /// </summary>
        public string LogFolderRelativePath
        {
            get
            {
                var folderRelativePath = Logger.LogBasePath;

                if (Configuration.EnableLogGrouping)
                {
                    if (Configuration.IsLogGroupNameVolatile)
                        UpdateLogGroupName();

                    if (!LogGroupName.IsNullOrEmpty())
                        folderRelativePath = Path.Combine(folderRelativePath, LogGroupName);
                }

                return folderRelativePath;
            }
        }

        /// <summary>
        /// Gets the log folder full path.
        /// </summary>
        public string LogFolderFullPath
        {
            get
            {
                var folderFullPath = Logger.LogBaseNativePath;

                if (Configuration.EnableLogGrouping)
                {
                    if (Configuration.IsLogGroupNameVolatile)
                        UpdateLogGroupName();

                    if (!LogGroupName.IsNullOrEmpty())
                        folderFullPath = Path.Combine(folderFullPath, LogGroupName);
                }

                return folderFullPath;
            }
        }

        /// <summary>
        /// Gets the log file full path.
        /// </summary>
        public string LogFileFullPath
        {
            get { return Path.Combine(Logger.LogBaseNativePath, GetLogFileRelativePath()); }
        }

        /// <summary>
        /// Gets or sets the log group name.
        /// </summary>
        protected string LogGroupName { get; set; }

        /// <summary>
        /// Gets or sets the log file name prefix.
        /// </summary>
        protected string LogFileNamePrefix { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerManager"/> class.
        /// </summary>
        public LoggerManager()
            : this(new LoggerConfiguration())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerManager"/> class.
        /// </summary>
        /// <param name="configuration">The logger manager configuration.</param>
        /// <exception cref="System.ArgumentNullException">configuration</exception>
        public LoggerManager(LoggerConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (!IsLoggerRegistered)
                MvxTrace.Warning("LoggerManager -> Logger is not registered");

            _configuration = configuration;

            UpdateLogEncryptionStatus();

            UpdateLogGroupName();
            UpdateLogFileNamePrefix();
        }

        #endregion

        #region Common Methods

        /// <summary>
        /// Updates the name of the log group based on the configuration
        /// If the LogGroupName in configuration is set, it assumes that value
        /// Ohterwise it will use the LogGroupFormat to create a new name for the LogGroupName
        /// </summary>
        public virtual void UpdateLogGroupName()
        {
            if (Configuration.EnableLogGrouping)
            {
                if (!Configuration.ForcedLogGroupName.IsNullOrEmpty())
                    LogGroupName = Configuration.ForcedLogGroupName;
                else
                    LogGroupName = Configuration.GetLogGroupName();
            }
            else
            {
                LogGroupName = string.Empty;
            }
        }

        /// <summary>
        /// Updates the prefix to be added to the log filename.
        /// If the ForcedLogFileNamePrefix in configuration is set, it assumes that value
        /// Ohterwise it will use the LogFileNamePrefixTemplate to create a new name for the LogFileNamePrefix
        /// </summary>
        public virtual void UpdateLogFileNamePrefix()
        {
            LogFileNamePrefix = Configuration.GetPrefix();
        }

        private string GetLogFileName()
        {
            if (Configuration.IsLogFileNamePrefixVolatile)
                UpdateLogFileNamePrefix();

            return LogFileNamePrefix + Configuration.LogFileName;
        }

        /// <summary>
        /// Sets the encryption status.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public void SetEncryptionEnabled(bool enabled)
        {
            Configuration.CanEncryptLogs = enabled;
            UpdateLogEncryptionStatus();
        }
        private void UpdateLogEncryptionStatus()
        {
            if (Configuration.CanEncryptLogs)
                Logger.ActivateEncryption(Configuration.LogPassword);
        }

        /// <summary>
        /// Gets the log file relative path
        /// </summary>
        /// <returns></returns>
        protected virtual string GetLogFileRelativePath()
        {
            var fileRelativePath = GetLogFileName();

            if (Configuration.EnableLogGrouping)
            {
                if (Configuration.IsLogGroupNameVolatile)
                    UpdateLogGroupName();

                if (!LogGroupName.IsNullOrEmpty())
                    fileRelativePath = Path.Combine(LogGroupName, fileRelativePath);
            }

            return fileRelativePath;
        }

        /// <summary>
        /// Determines whether the log type is enabled for this logger manager.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        public bool CanLog(LogTypeEnum logType)
        {
            return Configuration.LogLevel.Has(logType);
        }

        private string GetLogTag(string tag)
        {
            if (tag.IsNullOrEmpty())
                return Configuration.DefaultLogTag;

            return tag;
        }

        #endregion

        #region Log Methods

        /// <summary>
        /// Logs a message with debug level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public Task LogDebugAsync(string message, string tag = "")
        {
            return LogCommonAsync(LogTypeEnum.Debug, message, GetLogFileRelativePath(), tag);
        }

        /// <summary>
        /// Logs a message with information level.
        /// The message is obtained by a func that returns string
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task LogInfoAsync(Func<string> actionMessage, string tag = "")
        {
            return LogInfoAsync(actionMessage.Invoke(), tag);
        }

        /// <summary>
        /// Logs a message with information level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public Task LogInfoAsync(string message, string tag = "")
        {
            return LogCommonAsync(LogTypeEnum.Info, message, GetLogFileRelativePath(), tag);
        }

        /// <summary>
        /// Logs a message with warning level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public Task LogWarningAsync(string message, string tag = "")
        {
            return LogCommonAsync(LogTypeEnum.Warning, message, GetLogFileRelativePath(), tag);
        }

        /// <summary>
        /// Logs a message with error level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public Task LogErrorAsync(string message, string tag = "")
        {
            return LogCommonAsync(LogTypeEnum.Error, message, GetLogFileRelativePath(), tag);
        }

        /// <summary>
        /// Logs a message and an exception with error level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public Task LogErrorAsync(string message, Exception ex, string tag = "")
        {
            return LogCommonAsync(LogTypeEnum.Error, message, ex, GetLogFileRelativePath(), tag);
        }

        /// <summary>
        /// Logs an exception with error level.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public Task LogErrorAsync(Exception ex, string tag = "")
        {
            return LogCommonAsync(LogTypeEnum.Error, ex, GetLogFileRelativePath(), tag);
        }

        /// <summary>
        /// Logs a message with fatal level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public Task LogFatalAsync(string message, string tag = "")
        {
            return LogCommonAsync(LogTypeEnum.Fatal, message, GetLogFileRelativePath(), tag);
        }

        /// <summary>
        /// Logs a message and an exception with fatal level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public Task LogFatalAsync(string message, Exception ex, string tag = "")
        {
            return LogCommonAsync(LogTypeEnum.Fatal, message, ex, GetLogFileRelativePath(), tag);
        }

        /// <summary>
        /// Logs an exception with fatal level.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public Task LogFatalAsync(Exception ex, string tag = "")
        {
            return LogCommonAsync(LogTypeEnum.Fatal, ex, GetLogFileRelativePath(), tag);
        }

        /// <summary>
        /// Gets a list with the existing log files info.
        /// </summary>
        /// <returns>
        /// List of IFileInfo of the existing log files
        /// </returns>
        public Task<IEnumerable<IFileInfo>> GetExistingLogsAsync()
        {
            return Logger.GetExistingLogsAsync();
        }


        /// <summary>
        /// Common log method
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        protected virtual async Task LogCommonAsync(LogTypeEnum logType, string message, string logFileName, string tag)
        {
            if (CanLog(logType))
                await Logger.LogAsync(logType, GetLogTag(tag), message, logFileName);
        }

        /// <summary>
        /// Common log method
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        protected virtual async Task LogCommonAsync(LogTypeEnum logType, string message, Exception e, string logFileName, string tag)
        {
            if (CanLog(logType))
                await Logger.LogAsync(logType, GetLogTag(tag), message, e, logFileName);
        }

        /// <summary>
        /// Common log method
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="e">The e.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        protected virtual async Task LogCommonAsync(LogTypeEnum logType, Exception e, string logFileName, string tag)
        {
            if (CanLog(logType))
                await Logger.LogAsync(logType, GetLogTag(tag), e, logFileName);//, (Configuration.CanUseAnalyser && sendToAnalyzer));
        }

        /// <summary>
        /// Delete the expired logs with the possibility of maintaining the ones that have errors.
        /// (see logger manager configuraion)
        /// </summary>
        /// <returns>
        /// The count of deleted logs
        /// </returns>
        public virtual Task<int> PurgeExpiredLogsAsync()
        {
            return Logger.PurgeExpiredLogsAsync(Configuration.LogLifespan, Configuration.KeepLogswithErrors);
        }

        #endregion
    }
}
