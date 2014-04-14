 using System;
using Cirrious.CrossCore.Core;

namespace MvvmCrossUtilities.Plugins.Notification.ThreadRunners
{
    public class ThreadPoolActionRunner
        : IActionRunner
    {
        public void Run(Action action)
        {
            MvxAsyncDispatcher.BeginAsync(action);
        }
    }
}