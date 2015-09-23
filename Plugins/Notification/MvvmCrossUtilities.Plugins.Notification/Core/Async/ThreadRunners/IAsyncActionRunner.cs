using System;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Plugins.Notification.Core.Async.ThreadRunners
{
    /// <summary>
    /// Interface for an asynchronous action execution
    /// </summary>
    public interface IAsyncActionRunner
    {
        /// <summary>
        /// Executes the asynchronous action.
        /// </summary>
        /// <param name="action">The action.</param>
        Task RunAsync(Func<Task> action);
    }

    /// <summary>
    /// Interface for an asynchronous action execution with result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncActionRunner<T>
    {
        /// <summary>
        /// Executes the asynchronous action.
        /// </summary>
        /// <param name="action">The action.</param>
        Task<T> RunAsync(Func<Task<T>> action);
    }
}