using System;

namespace MvvmCrossUtilities.Plugins.Notification.ThreadRunners
{
    public interface IActionRunner
    {
        void Run(Action action);
    }
}