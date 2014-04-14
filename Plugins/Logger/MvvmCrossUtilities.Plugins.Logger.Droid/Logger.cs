using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.CrossCore;
using MvvmCrossUtilities.Plugins.Storage;

namespace MvvmCrossUtilities.Plugins.Logger.Droid
{
    public class Logger : ILogger
    {
        #region Constants

        private const string DEFAULT_LOG_FILE_EXTENSION = ".txt";
        private const string DEFAULT_LOG_FILENAME = "ApplicationLog";
        private const string DEFAULT_LOG_EXECUTIONTIME_FILENAME = "ExecutionTimeLog";
        private const string DEFAULT_LOGFOLDER = "logs";

        private const string DEFAULT_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss,fff";

        private const string DEFAULT_LOGENTRY_NORMAL_WITH_TAG = "[{0}] - #{1} - {2} - {3} - {4}";
        private const string DEFAULT_LOGENTRY_NORMAL_WITHOUT_TAG = "[{0}] - #{1} - {2} - {3}";

        private const string DEFAULT_LOGENTRY_EXCEPTION_WITH_TAG = "[{0}] - #{1} - {2} - {3}";
        private const string DEFAULT_LOGENTRY_EXCEPTION_WITHOUT_TAG = "[{0}] - #{1} - {2}";

        private const string DEFAULT_LOG_EXECUTIONTIME_TEMPLATE = @"TIME: {1:hh\:mm\:ss\:fffff} {0}";

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

        #endregion

        #region Constructor

        public Logger()
        {
        }

        #endregion

        #region ILogger Members

        public string LogBasePath
        {
            get { return DEFAULT_LOGFOLDER; }
        }

        public string LogBaseNativePath
        {
            get { return StorageManager.NativePath(StorageLocation.ExternalPublic, DEFAULT_LOGFOLDER); }
        }

        public void Log(LogLevel level, string tag, string message)
        {
            Log(level, tag, message, DEFAULT_LOG_FILENAME);
        }
        public void Log(LogLevel level, string tag, string message, string logFileName)
        {
            try
            {
                var logEntry = string.Empty;

                if (!string.IsNullOrEmpty(tag))
                {
                    logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITH_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                               System.Threading.Thread.CurrentThread.ManagedThreadId,
                                                                               GetLevel(level),
                                                                               GetTag(tag),
                                                                               message);
                }
                else
                {
                    logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITHOUT_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                                  System.Threading.Thread.CurrentThread.ManagedThreadId,
                                                                                  GetLevel(level),
                                                                                  message);
                }

                logEntry += Environment.NewLine;

                LogCommon(logFileName, logEntry);
            }
            catch (Exception ex)
            {
                Android.Util.Log.Wtf(this.GetType().Name, ex.Message);
                throw;
            }
        }

        public void Log(LogLevel level, string tag, Exception e)
        {
            Log(level, tag, e, DEFAULT_LOG_FILENAME);
        }
        public void Log(LogLevel level, string tag, Exception e, string logFileName)
        {
            try
            {
                var contents = new StringBuilder();
                var logEntry = string.Empty;

                if (!string.IsNullOrEmpty(tag))
                {
                    logEntry = string.Format(DEFAULT_LOGENTRY_EXCEPTION_WITH_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                                  System.Threading.Thread.CurrentThread.ManagedThreadId,
                                                                                  GetLevel(level),
                                                                                  GetTag(tag));
                }
                else
                {
                    logEntry = string.Format(DEFAULT_LOGENTRY_EXCEPTION_WITHOUT_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                                     System.Threading.Thread.CurrentThread.ManagedThreadId,
                                                                                     GetLevel(level));
                }

                contents.AppendLine(logEntry);

                WriteException(e, contents);
                contents.AppendLine();

                LogCommon(logFileName, contents.ToString());

            }
            catch (Exception ex)
            {
                Android.Util.Log.Wtf(this.GetType().Name, ex.Message);
                throw;
            }
        }

        public void Log(LogLevel level, string tag, string message, Exception e)
        {
            Log(level, tag, e, DEFAULT_LOG_FILENAME);
        }
        public void Log(LogLevel level, string tag, string message, Exception e, string logFileName)
        {
            try
            {
                var contents = new StringBuilder();
                var logEntry = string.Empty;

                if (!string.IsNullOrEmpty(tag))
                {
                    logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITH_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                               System.Threading.Thread.CurrentThread.ManagedThreadId,
                                                                               GetLevel(level),
                                                                               GetTag(tag),
                                                                               message);
                }
                else
                {
                    logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITHOUT_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                                  System.Threading.Thread.CurrentThread.ManagedThreadId,
                                                                                  GetLevel(level),
                                                                                  message);
                }

                contents.AppendLine(logEntry);

                WriteException(e, contents);
                contents.AppendLine();

                LogCommon(logFileName, contents.ToString());
            }
            catch (Exception ex)
            {
                Android.Util.Log.Wtf(this.GetType().Name, ex.Message);
                throw;
            }
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

                Log(LogLevel.Info, null, string.Format(DEFAULT_LOG_EXECUTIONTIME_TEMPLATE, methodName, executionTime), logFileName);
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

                Log(LogLevel.Info, null, string.Format(DEFAULT_LOG_EXECUTIONTIME_TEMPLATE, methodName(result), executionTime), logFileName);

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
        public IEnumerable<IFileInfo> GetExistingLogs()
        {
            return GetExistingLogs(null);
        }
        public IEnumerable<IFileInfo> GetExistingLogs(string relativeLogFolderPath)
        {
            var logsList = new List<IFileInfo>();
            var folderPath = DEFAULT_LOGFOLDER;
            if (!string.IsNullOrEmpty(relativeLogFolderPath))
            {
                folderPath = StorageManager.PathCombine(DEFAULT_LOGFOLDER, relativeLogFolderPath);
            }

            if (StorageManager.FolderExists(StorageLocation.ExternalPublic, folderPath))
            {
                logsList = StorageManager.GetFilesIn(StorageLocation.ExternalPublic, folderPath).ToList();
            }

            return logsList;
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
        }

        public void DeactiveEncryption()
        {
            EncryptionActivated = false;
            Password = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Common method for logging
        /// </summary>
        /// <param name="logFileName">Name of the log file.</param>
        /// <param name="contents">The contents.</param>
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

        /// <summary>
        /// Writes the exception.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="sb">The sb.</param>
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

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="defaultTag">The default tag.</param>
        /// <returns></returns>
        private string GetTag(string tag, string defaultTag = "none")
        {
            return string.IsNullOrEmpty(tag) ? defaultTag : tag;
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        private string GetLevel(LogLevel level)
        {
            return level.ToString().ToUpper();
        }

        #endregion
    }
}