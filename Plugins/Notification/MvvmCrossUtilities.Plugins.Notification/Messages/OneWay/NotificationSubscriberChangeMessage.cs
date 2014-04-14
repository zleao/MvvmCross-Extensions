using System;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Notification.Messages.OneWay
{
    public sealed class NotificationSubscriberChangeMessage : NotificationOneWayMessage
    {
        public Type MessageType { get; private set; }
        public int SubscriberCount { get; private set; }

        public NotificationSubscriberChangeMessage(object sender, Type messageType) 
            : this(sender, messageType, 0)
        {
        }

        public NotificationSubscriberChangeMessage(object sender, Type messageType, int countSubscribers) 
            : base(sender)
        {
            SubscriberCount = countSubscribers;
            MessageType = messageType;
        }
    }
}