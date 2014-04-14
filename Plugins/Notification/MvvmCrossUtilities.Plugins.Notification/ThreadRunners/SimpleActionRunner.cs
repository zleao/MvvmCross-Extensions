using System;

namespace MvvmCrossUtilities.Plugins.Notification.ThreadRunners
{
    public class SimpleActionRunner
        : IActionRunner
    {
        public void Run(Action action)
        {
            action();
        }
    }
}