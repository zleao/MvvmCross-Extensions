using MvxExtensions.Plugins.Logger;
using MvxExtensions.Plugins.Storage;
using MvvmCross.Platform.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvxExtensions.Libraries.Portable.Core.Services.Logger
{
    /// <summary>
    /// Class used to simulate a logger when there's none registered in the IOC
    /// </summary>
    public sealed class DummyLogger : ILogger
    {
        #region Constants

        private const string DEFAULT_LOG_FILE_EXTENSION = "";
        private const string DEFAULT_LOG_FILENAME = "";
        private const string DEFAULT_LOG_EXECUTIONTIME_FILENAME = "";
        private const string DEFAULT_LOGFOLDER = "";

        private const string DEFAULT_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss,fff";

        private const string DEFAULT_LOGENTRY_NORMAL_WITH_TAG = "[{0}] - #{1} - {2} - {3} - {4}";
        private const string DEFAULT_LOGENTRY_NORMAL_WITHOUT_TAG = "[{0}] - #{1} - {2} - {3}";

        private const string DEFAULT_LOGENTRY_EXCEPTION_WITH_TAG = "[{0}] - #{1} - {2} - {3}";
        private const string DEFAULT_LOGENTRY_EXCEPTION_WITHOUT_TAG = "[{0}] - #{1} - {2}";

        private const string DEFAULT_LOG_EXECUTIONTIME_TEMPLATE = @"TIME: {1} {0}";

        #endregion

        #region Properties

        private int CurrentThreadId 
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
        public string LogBasePath { get { return string.Empty; } }

        /// <summary>
        /// Gets the log base native path.
        /// </summary>
        /// <value>
        /// The log base native path.
        /// </value>
        public string LogBaseNativePath { get { return string.Empty; } }

        /// <summary>
        /// Gets a value indicating whether [encryption activated].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [encryption activated]; otherwise, <c>false</c>.
        /// </value>
        public bool EncryptionActivated { get { return false; } }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get { return string.Empty; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyLogger"/> class.
        /// </summary>
        public DummyLogger()
        {
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Activates the encryption.
        /// </summary>
        /// <param name="password">The password.</param>
        public void ActivateEncryption(string password)
        {
            //Do nothing
        }

        /// <summary>
        /// Deactives the encryption.
        /// </summary>
        public void DeactiveEncryption()
        {
            //Do nothing
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
        private string GetLevel(LogTypeEnum level)
        {
            return level.ToString().ToUpper();
        }

        #endregion

        #region Log Methods

        /// <summary>
        /// Logs the asynchronous.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum level, string tag, string message)
        {
            return LogAsync(level, tag, message, DEFAULT_LOG_FILENAME);
        }
        /// <summary>
        /// Logs the asynchronous.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum level, string tag, string message, string logFileName)
        {
            var logEntry = string.Empty;

            if (!string.IsNullOrEmpty(tag))
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITH_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                           CurrentThreadId,
                                                                           GetLevel(level),
                                                                           GetTag(tag),
                                                                           message);
            }
            else
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITHOUT_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                              CurrentThreadId,
                                                                              GetLevel(level),
                                                                              message);
            }

            logEntry += Environment.NewLine;

            return LogCommonAsync(logFileName, logEntry);
        }

        /// <summary>
        /// Logs the asynchronous.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum level, string tag, Exception e)
        {
            return LogAsync(level, tag, e, DEFAULT_LOG_FILENAME);
        }
        /// <summary>
        /// Logs the asynchronous.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="e">The e.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum level, string tag, Exception e, string logFileName)
        {
            var contents = new StringBuilder();
            var logEntry = string.Empty;

            if (!string.IsNullOrEmpty(tag))
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_EXCEPTION_WITH_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                              CurrentThreadId,
                                                                              GetLevel(level),
                                                                              GetTag(tag));
            }
            else
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_EXCEPTION_WITHOUT_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                                 CurrentThreadId,
                                                                                 GetLevel(level));
            }

            contents.AppendLine(logEntry);

            WriteException(e, contents);
            contents.AppendLine();

            return LogCommonAsync(logFileName, contents.ToString());
        }

        /// <summary>
        /// Logs the asynchronous.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum level, string tag, string message, Exception e)
        {
            return LogAsync(level, tag, e, DEFAULT_LOG_FILENAME);
        }
        /// <summary>
        /// Logs the asynchronous.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        public Task LogAsync(LogTypeEnum level, string tag, string message, Exception e, string logFileName)
        {
            var contents = new StringBuilder();
            var logEntry = string.Empty;

            if (!string.IsNullOrEmpty(tag))
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITH_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                           CurrentThreadId,
                                                                           GetLevel(level),
                                                                           GetTag(tag),
                                                                           message);
            }
            else
            {
                logEntry = string.Format(DEFAULT_LOGENTRY_NORMAL_WITHOUT_TAG, DateTime.Now.ToString(DEFAULT_DATETIME_FORMAT),
                                                                              CurrentThreadId,
                                                                              GetLevel(level),
                                                                              message);
            }

            contents.AppendLine(logEntry);

            WriteException(e, contents);
            contents.AppendLine();

            return LogCommonAsync(logFileName, contents.ToString());
        }

        /// <summary>
        /// Logs the method execution time asynchronous.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task LogMethodExecutionTimeAsync(Action method, Func<string> methodName)
        {
            return LogMethodExecutionTimeAsync(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the method execution time asynchronous.
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
        /// Logs the method execution time asynchronous.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task LogMethodExecutionTimeAsync(Action method, string methodName)
        {
            return LogMethodExecutionTimeAsync(method, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the method execution time asynchronous.
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
        /// Logs the method execution time asynchronous.
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
        /// Logs the method execution time asynchronous.
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

            return Task.FromResult<T>(default(T));
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
        /// Logs the method execution time asynchronous.
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

        /// <summary>
        /// Logs the asynchronous method execution time asynchronous.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, Func<string> methodName)
        {
            return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the asynchronous method execution time asynchronous.
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
        /// Logs the asynchronous method execution time asynchronous.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, string methodName)
        {
            return LogAsyncMethodExecutionTimeAsync(asyncMethod, methodName, DEFAULT_LOG_EXECUTIONTIME_FILENAME);
        }
        /// <summary>
        /// Logs the asynchronous method execution time asynchronous.
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
        /// Logs the asynchronous method execution time asynchronous.
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
        /// Logs the asynchronous method execution time asynchronous.
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

            return Task.FromResult<T>(default(T));
        }
        /// <summary>
        /// Logs the asynchronous method execution time asynchronous.
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
        /// Logs the asynchronous method execution time asynchronous.
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

        /// <summary>
        /// Gets the existing logs asynchronous.
        /// </summary>
        /// <param name="relativeLogFolderPath">The relative log folder path.</param>
        /// <returns></returns>
        public Task<IEnumerable<IFileInfo>> GetExistingLogsAsync(string relativeLogFolderPath = null)
        {
            return Task.FromResult(new List<IFileInfo>().AsEnumerable());
        }


        /// <summary>
        /// Common method for logging
        /// </summary>
        /// <param name="logFileName">Name of the log file.</param>
        /// <param name="contents">The contents.</param>
        private async Task LogCommonAsync(string logFileName, string contents)
        {
            await Task.Run(() => MvxTrace.Trace(contents));
        }


        /// <summary>
        /// Delete the expired logs with the possibility of maintaining the ones that have errors.
        /// </summary>
        /// <param name="logLifespan">The log lifespan.</param>
        /// <param name="keepLogsWithErrors">if set to <c>true</c> keep logs with errors.</param>
        /// <returns>
        /// The count of deleted logs
        /// </returns>
        public Task<int> PurgeExpiredLogsAsync(TimeSpan logLifespan, bool keepLogsWithErrors)
        {
            return Task.FromResult(0);
        }

        #endregion
    }
}
    