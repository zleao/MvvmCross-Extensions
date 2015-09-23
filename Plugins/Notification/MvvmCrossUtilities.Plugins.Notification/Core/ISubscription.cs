using System;

namespace MvvmCrossUtilities.Plugins.Notification.Core
{
    /// <summary>
    /// Generic subscription interface
    /// </summary>
    public interface ISubscription
    {
        /// <summary>
        /// Context of the notification.
        /// </summary>
        string Context { get; }

        /// <summary>
        /// Unique identificer of the subscription.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Indcates if the subscriber is still connected to this subscription instance
        /// </summary>
        bool IsAlive { get; }
    }
}
