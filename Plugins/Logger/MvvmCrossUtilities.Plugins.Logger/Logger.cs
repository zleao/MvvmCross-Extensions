using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using MvvmCrossUtilities.Plugins.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE
namespace MvvmCrossUtilities.Plugins.Logger.WindowsPhone
#elif WINDOWS_COMMON
namespace MvvmCrossUtilities.Plugins.Logger.WindowsCommon
#elif MONODROID
namespace MvvmCrossUtilities.Plugins.Logger.Droid
#else
namespace MvvmCrossUtilities.Plugins.Logger.WPF
#endif
{
    public class Logger : ILogger
    {
        #region Constants

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

        #region Properties

        protected IStorageManager StorageManager
        {
            get
            {
                return _storageManager ?? (_storageManager = Mvx.Resolve<IStorageManager>());
            }
        }
        protected IStorageManager _storageManager;

        protected int CurrentThreadId
        {
            get
            {
                return Task.CurrentId.GetValueOrDefault();
            }
        }

        #endregion

        #region Constructor

        public Logger()
        {
        }

        #endregion

        #region Common (Sync & Async)

        public string LogBasePath
        {
            get { return DEFAULT_LOGFOLDER; }
        }

        public string LogBaseNativePath
        {
            get { return StorageManager.NativePath(StorageLocation.ExternalPublic, DEFAULT_LOGFOLDER); }
        }

        public bool EncryptionActivated { get; private set; }

        public string Password { get; private set; }

        public void ActivateEncryption(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                Password = password;
                EncryptionActivated = true;
            }
            else
            {
                MvxTrace.Warning("Log encryption was not activated because password is empty");
            }
        }

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

        #region Synchronous

        public void Log(LogTypeEnum logType, string tag, string message)
        {
            Log(logType, tag, message, DEFAULT_LOG_FILENAME);
        }
        public void Log(LogTypeEnum logType, string tag, string message, string logFileName)
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

            LogCommon(logFileName, logEntry);
        }

        public void Log(LogTypeEnum logType, string tag, Exception e)
        {
            Log(logType, tag, e, DEFAULT_LOG_FILENAME);
        }
        public void Log(LogTypeEnum logType, string tag, Exception e, string logFileName)
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

            LogCommon(logFileName, contents.ToString());
        }

        public void Log(LogTypeEnum logType, string tag, string message, Exception e)
        {
            Log(logType, tag, e, DEFAULT_LOG_FILENAME);
        }
        public void Log(LogTypeEnum logType, string tag, string message, Exception e, string logFileName)
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

            LogCommon(logFileName, contents.ToString());
        }

        public void LogExecutionTime(Action method, Func<string> methodName)
        {
            LogExecutionTime(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        public void LogExecutionTime(Action method, Func<string> methodName, string logFileName)
        {
            if (methodName != null)
                LogExecutionTime(method, methodName(), logFileName);
        }
        public void LogExecutionTime(Action method, string methodName)
        {
            LogExecutionTime(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        public void LogExecutionTime(Action method, string methodName, string logFileName)
        {
            if (method != null)
            {
                var startDateTime = DateTime.Now;
                method.Invoke();
                var endDateTime = DateTime.Now;

                var executionTime = endDateTime - startDateTime;

                Log(LogTypeEnum.Info, null, string.Format(DEFAULT_LOG_EXECUTIONTIME_TEMPLATE, methodName, executionTime), logFileName);
            }
        }

        public T LogExecutionTime<T>(Func<T> method, string methodName)
        {
            return LogExecutionTime(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        public T LogExecutionTime<T>(Func<T> method, string methodName, string logFileName)
        {
            if (methodName != null)
                return LogExecutionTime(method, (ignore) => { return methodName; }, logFileName);

            return default(T);
        }
        public T LogExecutionTime<T>(Func<T> method, Func<T, string> methodName)
        {
            return LogExecutionTime(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        public T LogExecutionTime<T>(Func<T> method, Func<T, string> methodName, string logFileName)
        {
            if (method != null)
            {
                var startDateTime = DateTime.Now;
                var result = method.Invoke();
                var endDateTime = DateTime.Now;

                var executionTime = endDateTime - startDateTime;

                Log(LogTypeEnum.Info, null, string.Format(DEFAULT_LOG_EXECUTIONTIME_TEMPLATE, methodName(result), executionTime), logFileName);

                return result;
            }

            return default(T);
        }

        /// <summary>
        /// Gets the list of existing logs in the specified folder path.
        /// If 'relativeLogFolderPath' is null, it gets the logs in the root folder of logging
        /// </summary>
        /// <param name="relativeLogFolderPath">The relative log folder path.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<IFileInfo> GetExistingLogs(string relativeLogFolderPath = null)
        {
            var logsList = new List<IFileInfo>();
            var folderPath = DEFAULT_LOGFOLDER;
            if (!string.IsNullOrEmpty(relativeLogFolderPath))
            {
                folderPath = StorageManager.PathCombine(DEFAULT_LOGFOLDER, relativeLogFolderPath);
            }

            if (StorageManager.FolderExists(StorageLocation.ExternalPublic, folderPath))
            {
                logsList = StorageManager.GetFilesIn(StorageLocation.ExternalPublic, false, folderPath).ToList();
            }

            return logsList;
        }

        /// <summary>
        /// Common method for logging
        /// </summary>
        /// <param name="logFileName">Name of the log file.</param>
        /// <param name="contents">The contents.</param>
        /// 
        private void LogCommon(string logFileName, string contents)
        {
            var relativeLogPath = StorageManager.PathCombine(DEFAULT_LOGFOLDER, logFileName + DEFAULT_LOG_FILE_EXTENSION);

            if (EncryptionActivated)
            {
                StorageManager.WriteEncryptedFile(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, relativeLogPath, contents, Password);
            }
            else
            {
                StorageManager.WriteFile(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, relativeLogPath, contents);
            }
        }

        #endregion

        #region Asynchronous

        public Task LogAsync(LogTypeEnum logType, string tag, string message)
        {
            return LogAsync(logType, tag, message, DEFAULT_LOG_FILENAME);
        }
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

            return LogCommonAsync(logFileName, logEntry);
        }

        public Task LogAsync(LogTypeEnum logType, string tag, Exception e)
        {
            return LogAsync(logType, tag, e, DEFAULT_LOG_FILENAME);
        }
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

            return LogCommonAsync(logFileName, contents.ToString());
        }

        public Task LogAsync(LogTypeEnum logType, string tag, string message, Exception e)
        {
            return LogAsync(logType, tag, e, DEFAULT_LOG_FILENAME);
        }
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

            return LogCommonAsync(logFileName, contents.ToString());
        }

        public Task LogMethodExecutionTimeAsync(Action method, Func<string> methodName)
        {
            return LogMethodExecutionTimeAsync(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        public Task LogMethodExecutionTimeAsync(Action method, Func<string> methodName, string logFileName)
        {
            if (methodName != null)
                return LogMethodExecutionTimeAsync(method, methodName(), logFileName);

            return null;
        }
        public Task LogMethodExecutionTimeAsync(Action method, string methodName)
        {
            return LogMethodExecutionTimeAsync(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
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

        public Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, string methodName)
        {
            return LogMethodExecutionTimeAsync(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        public Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, string methodName, string logFileName)
        {
            if (methodName != null)
                return LogMethodExecutionTimeAsync(method, (ignore) => { return methodName; }, logFileName);

            return Task.FromResult<T>(default(T));
        }
        public Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, Func<T, string> methodName)
        {
            return LogMethodExecutionTimeAsync(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        public async Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, Func<T, string> methodName, string logFileName)
        {
            if (method != null)
            {
                T result = default(T);

                var startDateTime = DateTime.Now;
                result = method.Invoke();
                var endDateTime = DateTime.Now;

                var executionTime = endDateTime - startDateTime;

                await LogAsync(LogTypeEnum.Info, null, string.Format(DEFAULT_LOG_EXECUTIONTIME_TEMPLATE, methodName(result), executionTime), logFileName);

                return result;
            }

            return default(T);
        }

        public Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, Func<string> methodName)
        {
            return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        public Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, Func<string> methodName, string logFileName)
        {
            if (methodName != null)
                return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName(), logFileName);

            return null;
        }
        public Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, string methodName)
        {
            return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
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

        public Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, string methodName)
        {
            return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        public Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, string methodName, string logFileName)
        {
            if (methodName != null)
                return LogAsyncMethodExecutionTimeAsync<T>(asyncMethod, (ignore) => { return methodName; }, logFileName);

            return Task.FromResult<T>(default(T));
        }
        public Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, Func<T, string> methodName)
        {
            return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        public async Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, Func<T, string> methodName, string logFileName)
        {
            if (asyncMethod != null)
            {
                T result = default(T);

                var startDateTime = DateTime.Now;
                result = await asyncMethod.Invoke();
                var endDateTime = DateTime.Now;

                var executionTime = endDateTime - startDateTime;

                await LogAsync(LogTypeEnum.Info, null, string.Format(DEFAULT_LOG_EXECUTIONTIME_TEMPLATE, methodName(result), executionTime), logFileName);

                return result;
            }

            return default(T);
        }

        public async Task<IEnumerable<IFileInfo>> GetExistingLogsAsync(string relativeLogFolderPath = null)
        {
            var logsList = new List<IFileInfo>();
            var folderPath = DEFAULT_LOGFOLDER;
            if (!string.IsNullOrEmpty(relativeLogFolderPath))
            {
                folderPath = StorageManager.PathCombine(DEFAULT_LOGFOLDER, relativeLogFolderPath);
            }

            if (await StorageManager.FolderExistsAsync(StorageLocation.ExternalPublic, folderPath))
            {
                logsList = (await StorageManager.GetFilesInAsync(StorageLocation.ExternalPublic, false, folderPath)).ToList();
            }

            return logsList;
        }


        private async Task LogCommonAsync(string logFileName, string contents, Exception ex = null)
        {
            var relativeLogPath = StorageManager.PathCombine(DEFAULT_LOGFOLDER, logFileName + DEFAULT_LOG_FILE_EXTENSION);

            if (EncryptionActivated)
            {
                await StorageManager.WriteEncryptedFileAsync(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, relativeLogPath, contents, Password);
            }
            else
            {
                await StorageManager.WriteFileAsync(StorageLocation.ExternalPublic, StorageMode.CreateOrAppend, relativeLogPath, contents);
            }
        }

        #endregion
    }
}
