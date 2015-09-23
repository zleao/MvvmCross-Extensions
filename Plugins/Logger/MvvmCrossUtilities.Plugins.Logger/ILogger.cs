using MvvmCrossUtilities.Plugins.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Plugins.Logger
{
    /// <summary>
    /// Interface for the logger plugin
    /// </summary>
    public interface ILogger
    {
        #region Common (Sync & Async)

        /// <summary>
        /// Gets the log base path.
        /// </summary>
        /// <value>
        /// The log base path.
        /// </value>
        string LogBasePath { get; }

        /// <summary>
        /// Gets the log base native path.
        /// </summary>
        /// <value>
        /// The log base native path.
        /// </value>
        string LogBaseNativePath { get; }

        /// <summary>
        /// Gets a value indicating whether encryption activated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if encryption activated; otherwise, <c>false</c>.
        /// </value>
        bool EncryptionActivated { get; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        string Password { get; }

        /// <summary>
        /// Activates the encryption to all logs.
        /// </summary>
        /// <param name="password">The password.</param>
        void ActivateEncryption(string password);

        /// <summary>
        /// Deactives the encryption.
        /// </summary>
        void DeactiveEncryption();
        
        #endregion

        #region Asynchronous

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Task LogAsync(LogTypeEnum logType, string tag, string message);
        /// <summary>
        /// Logs  a message.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        Task LogAsync(LogTypeEnum logType, string tag, string message, string logFileName);

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        Task LogAsync(LogTypeEnum logType, string tag, Exception e);
        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="e">The e.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        Task LogAsync(LogTypeEnum logType, string tag, Exception e, string logFileName);

        /// <summary>
        /// Logs a message and an exception
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        Task LogAsync(LogTypeEnum logType, string tag, string message, Exception e);
        /// <summary>
        /// Logs a message and an exception.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        Task LogAsync(LogTypeEnum logType, string tag, string message, Exception e, string logFileName);


        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        Task LogMethodExecutionTimeAsync(Action method, Func<string> methodName);
        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        Task LogMethodExecutionTimeAsync(Action method, Func<string> methodName, string logFileName);
        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        Task LogMethodExecutionTimeAsync(Action method, string methodName);
        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        Task LogMethodExecutionTimeAsync(Action method, string methodName, string logFileName);

        /// <summary>
        /// Logs the method execution time asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, Func<T, string> methodName);
        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, Func<T, string> methodName, string logFileName);
        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, string methodName);
        /// <summary>
        /// Logs the method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        Task<T> LogMethodExecutionTimeAsync<T>(Func<T> method, string methodName, string logFileName);

        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, Func<string> methodName);
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, Func<string> methodName, string logFileName);
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, string methodName);
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        Task LogAsyncMethodExecutionTimeAsync(Func<Task> asyncMethod, string methodName, string logFileName);

        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, Func<T, string> methodName);
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, Func<T, string> methodName, string logFileName);
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, string methodName);
        /// <summary>
        /// Logs the asynchronous method execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncMethod">The asynchronous method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logFileName">Name of the log file.</param>
        /// <returns></returns>
        Task<T> LogAsyncMethodExecutionTimeAsync<T>(Func<Task<T>> asyncMethod, string methodName, string logFileName);


        /// <summary>
        /// Gets the list of existing logs in the specified folder path.
        /// If 'relativeLogFolderPath' is null, it gets the logs in the root folder of logging
        /// </summary>
        /// <param name="relativeLogFolderPath">The relative log folder path.</param>
        /// <returns></returns>
        Task<IEnumerable<IFileInfo>> GetExistingLogsAsync(string relativeLogFolderPath = null);
        
        #endregion
    }
}
