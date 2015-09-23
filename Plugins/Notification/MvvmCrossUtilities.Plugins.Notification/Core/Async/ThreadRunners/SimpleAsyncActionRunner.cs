using System;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Plugins.Notification.Core.Async.ThreadRunners
{
    /// <summary>
    /// Runner for asynchronous actions
    /// </summary>
    public class SimpleAsyncActionRunner : IAsyncActionRunner
    {
        /// <summary>
        /// Executes the asynchronous action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public Task RunAsync(Func<Task> action)
        {
            return action.Invoke();
        }
    }

    /// <summary>
    /// Runner for asynchronous actions with result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleAsyncActionRunner<T> : IAsyncActionRunner<T>
    {
        /// <summary>
        /// Executes the asynchronous action with result.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public Task<T> RunAsync(Func<Task<T>> action)
        {
            return action.Invoke();
        }
    }
}