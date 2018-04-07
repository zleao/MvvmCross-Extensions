using MvxExtensions.Plugins.Logger;
using MvxExtensions.Plugins.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvxExtensions.Libraries.Portable.Core.Services.Logger
{
    /// <summary>
    /// Interface for the logger manager
    /// </summary>
    public interface ILoggerManager
    {
        #region Properties

        /// <summary>
        /// Gets the log folder relative path.
        /// </summary>
        string LogFolderRelativePath { get; }

        /// <summary>
        /// Gets the log folder full path.
        /// </summary>
        string LogFolderFullPath { get; }

        /// <summary>
        /// Gets the log file full path.
        /// </summary>
        string LogFileFullPath { get; }

        #endregion

        #region Common Methods

        /// <summary>
        /// Updates the name of the log group based on the configuration
        /// If the ForcedLogGroupName in configuration is set, it assumes that value
        /// Ohterwise it will use the LogGroupTemplate to create a new name for the LogGroupName
        /// </summary>
        void UpdateLogGroupName();

        /// <summary>
        /// Updates the prefix to be added to the log filename.
        /// If the ForcedLogFileNamePrefix in configuration is set, it assumes that value
        /// Ohterwise it will use the LogFileNamePrefixTemplate to create a new name for the LogFileNamePrefix
        /// </summary>
        void UpdateLogFileNamePrefix();

        /// <summary>
        /// Sets the encryption status.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        void SetEncryptionEnabled(bool enabled);

        /// <summary>
        /// Determines whether the log type is enabled for this logger manager.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        bool CanLog(LogTypeEnum logType);

        #endregion

        #region Log Methods

        /// <summary>
        /// Logs a message with debug level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        Task LogDebugAsync(string message, string tag = "");

        /// <summary>
        /// Logs a message with information level.
        /// The message is obtained by a func that returns string
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="tag">The tag.</param>
        Task LogInfoAsync(Func<string> actionMessage, string tag = "");

        /// <summary>
        /// Logs a message with information level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        Task LogInfoAsync(string message, string tag = "");

        /// <summary>
        /// Logs a message with warning level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        Task LogWarningAsync(string message, string tag = "");

        /// <summary>
        /// Logs a message with error level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        Task LogErrorAsync(string message, string tag = "");

        /// <summary>
        /// Logs a message and an exception with error level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        Task LogErrorAsync(string message, Exception ex, string tag = "");

        /// <summary>
        /// Logs an exception with error level.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="tag">The tag.</param>
        Task LogErrorAsync(Exception ex, string tag = "");

        /// <summary>
        /// Logs a message with fatal level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        Task LogFatalAsync(string message, string tag = "");

        /// <summary>
        /// Logs a message and an exception with fatal level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        Task LogFatalAsync(string message, Exception ex, string tag = "");

        /// <summary>
        /// Logs an exception with fatal level.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="tag">The tag.</param>
        Task LogFatalAsync(Exception ex, string tag = "");

        /// <summary>
        /// Gets a list with the existing log files info.
        /// </summary>
        /// <returns>List of IFileInfo of the existing log files</returns>
        Task<IEnumerable<IFileInfo>> GetExistingLogsAsync();

        /// <summary>
        /// Delete the expired logs with the possibility of maintaining the ones that have errors.
        /// (see logger manager configuraion)
        /// </summary>
        /// <returns>
        /// The count of deleted logs
        /// </returns>
        Task<int> PurgeExpiredLogsAsync();

        #endregion
    }
}
