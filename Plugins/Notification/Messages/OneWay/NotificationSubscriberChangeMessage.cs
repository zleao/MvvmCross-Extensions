using MvxExtensions.Plugins.Notification.Messages.Base;
using System;

namespace MvxExtensions.Plugins.Notification.Messages.OneWay
{
    /// <summary>
    /// Notification used to broadcast subscription changes
    /// </summary>
    public sealed class NotificationSubscriberChangeMessage : NotificationOneWayMessage
    {
        /// <summary>
        /// Type of the message involved in the subscription changes.
        /// </summary>
        public Type MessageType { get; private set; }

        /// <summary>
        /// Subscriber notification count for the specified type.
        /// </summary>
        public int SubscriberCount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSubscriberChangeMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="messageType">Type of the message.</param>
        public NotificationSubscriberChangeMessage(object sender, Type messageType) 
            : this(sender, messageType, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSubscriberChangeMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="countSubscribers">The count subscribers.</param>
        public NotificationSubscriberChangeMessage(object sender, Type messageType, int countSubscribers) 
            : base(sender)
        {
            SubscriberCount = countSubscribers;
            MessageType = messageType;
        }
    }
}