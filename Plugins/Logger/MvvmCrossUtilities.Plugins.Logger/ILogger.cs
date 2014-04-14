using System;
using System.Collections.Generic;
using MvvmCrossUtilities.Plugins.Storage;

namespace MvvmCrossUtilities.Plugins.Logger
{
    public interface ILogger
    {
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
        /// Logs a message with the specified tag
        /// </summary>
        void Log(LogLevel level, string tag, string message);
        void Log(LogLevel level, string tag, string message, string logFileName);

        /// <summary>
        /// Logs an exception with the specified tag
        /// </summary>
        void Log(LogLevel level, string tag, Exception e);
        void Log(LogLevel level, string tag, Exception e, string logFileName);

        /// <summary>
        /// Logs a message and an exception with the specified tag
        /// </summary>
        void Log(LogLevel level, string tag, string message, Exception e);
        void Log(LogLevel level, string tag, string message, Exception e, string logFileName);


        /// <summary>
        /// Logs the execution time.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        void LogExecutionTime(Action method, Func<string> methodName);
        void LogExecutionTime(Action method, Func<string> methodName, string logFileName);

        /// <summary>
        /// Logs the execution time.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        void LogExecutionTime(Action method, string methodName);
        void LogExecutionTime(Action method, string methodName, string logFileName);

        /// <summary>
        /// Logs the execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        T LogExecutionTime<T>(Func<T> method, Func<T, string> methodName);
        T LogExecutionTime<T>(Func<T> method, Func<T, string> methodName, string logFileName);

        /// <summary>
        /// Logs the execution time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        T LogExecutionTime<T>(Func<T> method, string methodName);
        T LogExecutionTime<T>(Func<T> method, string methodName, string logFileName);


        /// <summary>
        /// Gets the list of existing logs in the specified folder path.
        /// If 'relativeLogFolderPath' is null, it gets the logs in the root folder of logging
        /// </summary>
        /// <param name="relativeLogFolderPath">The relative log folder path.</param>
        /// <returns></returns>
        IEnumerable<IFileInfo> GetExistingLogs();
        IEnumerable<IFileInfo> GetExistingLogs(string relativeLogFolderPath);


        bool EncryptionActivated { get; }

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
    }
}
