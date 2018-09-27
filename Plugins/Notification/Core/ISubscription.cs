using System;

namespace MvxExtensions.Plugins.Notification.Core
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
        /// Indicates if the subscriber is still connected to this subscription instance
        /// </summary>
        bool IsAlive { get; }
    }
}
