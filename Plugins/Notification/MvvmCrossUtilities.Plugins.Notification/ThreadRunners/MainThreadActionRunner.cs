﻿using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.Platform;
using System;

namespace MvvmCrossUtilities.Plugins.Notification.ThreadRunners
{
    public class MainThreadActionRunner
        : IActionRunner
    {
        public void Run(Action action)
        {
            var dispatcher = MvxMainThreadDispatcher.Instance;
            if (dispatcher == null)
            {
                MvxTrace.Warning( "Not able to deliver message - no ui thread dispatcher available");
                return;
            }
            dispatcher.RequestMainThreadAction(action);
        }
    }
}