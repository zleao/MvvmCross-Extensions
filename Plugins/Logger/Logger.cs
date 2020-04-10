using MvvmCross.Base;
using MvvmCross.Logging;
using MvxExtensions.Plugins.Storage;
using MvxExtensions.Plugins.Storage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Logger
{
    /// <summary>
    /// Implementation of the ILogger
    /// </summary>
    /// <seealso cref="MvxExtensions.Plugins.Logger.ILogger" />
    public class Logger : ILogger
    {
        #region Constants

        private const string DEFAULT_LOGINFO_FILE_EXTENSION = ".info";
        private const string DEFAULT_LOG_FILE_EXTENSION = ".txt";
        private const string DEFAULT_LOG_FILENAME = "ApplicationLog";
        private const string DEFAULT_LOG_EXECUTIONTIME_FILENAME = "ExecutionTimeLog";
        private const string DEFAULT_LOGFOLDER = "Logs";
        private const string DEFAULT_ANALYSER_LOG_FILENAME = "AnalyserLog";

        private const string DEFAULT_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss,fff";

        private const string DEFAULT_LOGENTRY_NORMAL_WITH_TAG = "[{0}] - #{1:00000} - {2} - {3} - {4}";
        private const string DEFAULT_LOGENTRY_NORMAL_WITHOUT_TAG = "[{0}] - #{1:00000} - {2} - {3}";

        private const string DEFAULT_LOGENTRY_EXCEPTION_WITH_TAG = "[{0}] - #{1:00000} - {2} - {3}";
        private const string DEFAULT_LOGENTRY_EXCEPTION_WITHOUT_TAG = "[{0}] - #{1:00000} - {2}";

        private const string DEFAULT_LOG_EXECUTIONTIME_TEMPLATE = @"TIME: {1} {0}";

        #endregion

        #region Fields

        private readonly IStorageManager _storageManager;
        private readonly IStorageEncryptionManager _storageEncryptionManager;
        private readonly IMvxJsonConverter _jsonConverter;

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets the current thread identifier.
        /// </summary>
        /// <value>
        /// The current thread identifier.
        /// </value>
        protected int CurrentThreadId
        {
            get
            {
                return Task.CurrentId.GetValueOrDefault();
            }
        }


        /// <summary>
        /// Gets the log base path.
        /// </summary>
        /// <value>
        /// The log base path.
        /// </value>
        public string LogBasePath
        {
            get { return DEFAULT_LOGFOLDER; }
        }

        /// <summary>
        /// Gets the log base native path.
        /// </summary>
        /// <value>
        /// The log base native path.
        /// </value>
        public string LogBaseNativePath
        {
            get { return _storageManager.NativePath(StorageLocation.SharedDataDirectory, DEFAULT_LOGFOLDER); }
        }

        /// <summary>
        /// Gets a value indicating whether encryption activated.
        /// </summary>
        /// <value>
        /// <c>true</c> if encryption activated; otherwise, <c>false</c>.
        /// </value>
        public bool EncryptionActivated { get; private set; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger" /> class.
        /// </summary>
        /// <param name="storageManager">The storage manager.</param>
        /// <param name="storageEncryptionManager"></param>
        /// <param name="jsonConverter">The json converter.</param>
        public Logger(IStorageManager storageManager, IStorageEncryptionManager storageEncryptionManager, IMvxJsonConverter jsonConverter)
        {
            _storageManager = storageManager;
            _storageEncryptionManager = storageEncryptionManager;
            _jsonConverter = jsonConverter;
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Activates the encryption to all logs.
        /// </summary>
        /// <param name="password">The password.</param>
        public void ActivateEncryption(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                Password = password;
                EncryptionActivated = true;
            }
            else
            {
                MvxPluginLog.Instance.Warn("Log encryption was not activated because password is empty");
            }
        }

        /// <summary>
        /// Deactives the encryption.
        /// </summary>
        public void DeactiveEncryption()
        {
            EncryptionActivated = false;
            Password = null;
        }


        private void WriteException(Exception e, StringBuilder sb)
        {
            sb.AppendLine(e.GetType().ToString());
            sb.AppendLine(e.Message);
            sb.AppendLine(e.StackTrace);
            if (e.InnerException != null)
            {
                sb.AppendLine("- Inner Exception -");
                WriteException(e.InnerException, sb);
            }
        }

        private string GetTag(string tag, string defaultTag = "none")
        {
            return string.IsNullOrEmpty(tag) ? defaultTag : tag;
        }

        private string ConvertToString(LogTypeEnum level)
        {
            return level.ToString().ToUpper();
        }

        #endregion

        #region Log Methods

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum logType, string tag, string message)
        {
            return LogAsync(logType, tag, message, DEFAULT_LOG_FILENAME);
        }
        /// <summary>
        /// Logs  a message.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum logType, string tag, string message, string logFileName)
        {
            var logEntry = string.Empty;

            if (!string.IsNullOrEmpty(tag))
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITH_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                           CurrentThreadId,
                                                                           ConvertToString(logType),
                                                                           GetTag(tag),
                                                                           message);
            }
            else
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITHOUT_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                              CurrentThreadId,
                                                                              ConvertToString(logType),
                                                                              message);
            }

            logEntry += Environment.NewLine;

            return LogCommonAsync(logType, logFileName, logEntry);
        }

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum logType, string tag, Exception e)
        {
            return LogAsync(logType, tag, e, DEFAULT_LOG_FILENAME);
        }
        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="e">The e.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum logType, string tag, Exception e, string logFileName)
        {
            var contents = new StringBuilder();
            var logEntry = string.Empty;

            if (!string.IsNullOrEmpty(tag))
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_EXCEPTION_WITH_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                              CurrentThreadId,
                                                                              ConvertToString(logType),
                                                                              GetTag(tag));
            }
            else
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_EXCEPTION_WITHOUT_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                                 CurrentThreadId,
                                                                                 ConvertToString(logType));
            }

            contents.AppendLine(logEntry);

            WriteException(e, contents);
            contents.AppendLine();

            return LogCommonAsync(logType, logFileName, contents.ToString());
        }

        /// <summary>
        /// Logs a message and an exception
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum logType, string tag, string message, Exception e)
        {
            return LogAsync(logType, tag, e, DEFAULT_LOG_FILENAME);
        }
        /// <summary>
        /// Logs a message and an exception.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum logType, string tag, string message, Exception e, string logFileName)
        {
            var contents = new StringBuilder();
            var logEntry = string.Empty;

            if (!string.IsNullOrEmpty(tag))
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITH_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                           CurrentThreadId,
                                                                           ConvertToString(logType),
                                                                           GetTag(tag),
                                                                           message);
            }
            else
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITHOUT_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                              CurrentThreadId,
                                                                              ConvertToString(logType),
                                                                              message);
            }

            contents.AppendLine(logEntry);

            WriteException(e, contents);
            contents.AppendLine();

            return LogCommonAsync(logType, logFileName, contents.ToString());
        }

        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task LogMethodExecutionTimeAsync(Action method, Func<string> methodName)
        {
            return LogMethodExecutionTimeAsync(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public Task LogMethodExecutionTimeAsync(Action method, Func<string> methodName, string logFileName)
        {
            if (methodName != null)
                return LogMethodExecutionTimeAsync(method, methodName(), logFileName);

            return null;
        }
        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task LogMethodExecutionTimeAsync(Action method, string methodName)
        {
            return LogMethodExecutionTimeAsync(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public Task LogMethodExecutionTimeAsync(Action method, string methodName, string logFileName)
        {
            if (method != null)
            {
                var startDateTime = DateTime.Now;
                method.Invoke();
                var endDateTime = DateTime.Now;

                var executionTime = endDateTime - startDateTime;

                return LogAsync(LogTypeEnum.Info, null, string.Format(DEFAULT_LOG_EXECUTIONTIME_TEMPLATE, methodName, executionTime));
            }

            return null;
        }

        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, string methodName)
        {
            return LogMethodExecutionTimeAsync(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, string methodName, string logFileName)
        {
            if (methodName != null)
                return LogMethodExecutionTimeAsync(method, (ignore) => { return methodName; }, logFileName);

            return Task.FromResult<T>(default);
        }
        /// <summary>
        /// Logs the method execution time asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, Func<T, string> methodName)
        {
            return LogMethodExecutionTimeAsync(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public async Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, Func<T, string> methodName, string logFileName)
        {
            if (method != null)
            {
                T result = default;

                var startDateTime = DateTime.Now;
                result = method.Invoke();
                var endDateTime = DateTime.Now;

                var executionTime = endDateTime - startDateTime;

                await LogAsync(LogTypeEnum.Info, null, string.Format(DEFAULT_LOG_EXECUTIONTIME_TEMPLATE, methodName(result), executionTime), logFileName);

                return result;
            }

            return default;
        }

        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, Func<string> methodName)
        {
            return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, Func<string> methodName, string logFileName)
        {
            if (methodName != null)
                return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName(), logFileName);

            return null;
        }
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, string methodName)
        {
            return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public async Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, string methodName, string logFileName)
        {
            if (asyncMethod != null)
            {
                var startDateTime = DateTime.Now;
                await asyncMethod.Invoke();
                var endDateTime = DateTime.Now;

                var executionTime = endDateTime - startDateTime;

                await LogAsync(LogTypeEnum.Info, null, string.Format(DEFAULT_LOG_EXECUTIONTIME_TEMPLATE, methodName, executionTime), logFileName);
            }
        }

        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, string methodName)
        {
            return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, string methodName, string logFileName)
        {
            if (methodName != null)
                return LogAsyncMethodExecutionTimeAsync<T>(asyncMethod, (ignore) => { return methodName; }, logFileName);

            return Task.FromResult<T>(default);
        }
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, Func<T, string> methodName)
        {
            return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public async Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, Func<T, string> methodName, string logFileName)
        {
            if (asyncMethod != null)
            {
                T result = default;

                var startDateTime = DateTime.Now;
                result = await asyncMethod.Invoke();
                var endDateTime = DateTime.Now;

                var executionTime = endDateTime - startDateTime;

                await LogAsync(LogTypeEnum.Info, null, string.Format(DEFAULT_LOG_EXECUTIONTIME_TEMPLATE, methodName(result), executionTime), logFileName);

                return result;
            }

            return default;
        }

        /// <summary>
        /// Gets the list of existing logs in the specified folder path.
        /// If 'relativeLogFolderPath' is null, it gets the logs in the root folder of logging
        /// </summary>
        /// <param name="relativeLogFolderPath">The relative log folder path.</param>
        /// <returns></returns>
        public async Task<IEnumerable<IFileInfo>> GetExistingLogsAsync(string relativeLogFolderPath = null)
        {
            var logsList = new List<IFileInfo>();
            var folderPath = DEFAULT_LOGFOLDER;
            if (!string.IsNullOrEmpty(relativeLogFolderPath))
            {
                folderPath = _storageManager.PathCombine(DEFAULT_LOGFOLDER, relativeLogFolderPath);
            }

            if (await _storageManager.FolderExistsAsync(StorageLocation.SharedDataDirectory, folderPath))
            {
                logsList = (await _storageManager.GetFilesInAsync(StorageLocation.SharedDataDirectory, false, folderPath)).ToList();
            }

            return logsList;
        }


        /// <summary>
        /// Delete the expired logs with the possibility of maintaining the ones that have errors.
        /// </summary>
        /// <param name="logLifespan">The log lifespan.</param>
        /// <param name="keepLogsWithErrors">if set to <c>true</c> keep logs with errors.</param>
        /// <returns>
        /// The count of deleted logs
        /// </returns>
        public async Task<int> PurgeExpiredLogsAsync(TimeSpan logLifespan, bool keepLogsWithErrors)
        {
            var currentDate = DateTimeOffset.Now;
            var deletedLogFilesCounter = 0;

            var logFiles = await _storageManager.GetFilesInAsync(StorageLocation.SharedDataDirectory, true, DEFAULT_LOGFOLDER, DEFAULT_LOG_FILE_EXTENSION, SearchMode.EndsWith);
            foreach (var log in logFiles)
            {
                var deleteFile = false;

                var logFullPath = log.FileFullPath;

                var logDetails = await GetLogDetailsAsync(logFullPath);
                if (logDetails != null)
                {
                    //Use the details to check if the log has to be deleted
                    if (logDetails.CreationDate.Add(logLifespan) <= currentDate) //expired
                    {
                        var hasErrors = (logDetails.ErrorLogCount > 0 || logDetails.FatalLogCount > 0);
                        deleteFile = !(hasErrors && keepLogsWithErrors);
                    }
                }
                else
                {
                    //No details found.
                    //Probably some older log file that was built before version 4 of logger plugin.
                    //In these cases, the log will allways be deleted
                    deleteFile = true;
                }

                if (deleteFile)
                {
                    await _storageManager.DeleteFileAsync(logFullPath);
                    if (logDetails != null)
                        await _storageManager.DeleteFileAsync(GetLogDetailsPath(logFullPath));

                    deletedLogFilesCounter++;
                }
            }

            return deletedLogFilesCounter;
        }


        private async Task LogCommonAsync(LogTypeEnum logType, string logFileName, string contents, Exception ex = null)
        {
            var logRelativePath = _storageManager.PathCombine(DEFAULT_LOGFOLDER, logFileName + DEFAULT_LOG_FILE_EXTENSION);
            var logFullPath = _storageManager.NativePath(StorageLocation.SharedDataDirectory, logRelativePath);

            if (EncryptionActivated)
            {
                await _storageEncryptionManager.WriteEncryptedFileAsync(StorageMode.CreateOrAppend, logFullPath, contents, Password);
            }
            else
            {
                await _storageManager.WriteFileAsync(StorageMode.CreateOrAppend, logFullPath, contents);
            }

            await WriteLogInfoAsync(logType, logFullPath);
        }

        private async Task WriteLogInfoAsync(LogTypeEnum logType, string logFullPath)
        {
            var logDetails = await GetLogDetailsAsync(logFullPath);
            if (logDetails == null)
                logDetails = new LogDetails() { CreationDate = DateTimeOffset.Now };

            logDetails.IncrementLogCounter(logType);

            var contents = _jsonConverter.SerializeObject(logDetails);
            var logInfoFullPath = GetLogDetailsPath(logFullPath);

            if (EncryptionActivated)
            {
                await _storageEncryptionManager.WriteEncryptedFileAsync(StorageMode.Create, logInfoFullPath, contents, Password);
            }
            else
            {
                await _storageManager.WriteFileAsync(StorageMode.Create, logInfoFullPath, contents);
            }
        }

        private async Task<LogDetails> GetLogDetailsAsync(string logFullPath)
        {
            LogDetails logDetails = null;

            var logInfoFullPath = GetLogDetailsPath(logFullPath);

            if (await _storageManager.FileExistsAsync(logInfoFullPath))
            {
                string existingLogDetailsString = null;
                if (EncryptionActivated)
                    existingLogDetailsString = await _storageEncryptionManager.TryReadTextEncryptedFileAsync(logInfoFullPath, Password);
                if (string.IsNullOrEmpty(existingLogDetailsString))
                    existingLogDetailsString = await _storageManager.TryReadTextFileAsync(logInfoFullPath);

                if (!string.IsNullOrEmpty(existingLogDetailsString))
                {
                    try
                    {
                        var existingLogDetails = _jsonConverter.DeserializeObject<LogDetails>(existingLogDetailsString);
                        if (existingLogDetails != null)
                            logDetails = existingLogDetails;
                    }
                    catch
                    {
                        //ignore exception
                    }
                }
            }

            return logDetails;
        }

        private string GetLogDetailsPath(string logPath)
        {
            return logPath + DEFAULT_LOGINFO_FILE_EXTENSION;
        }
      
        #endregion
    }
}
