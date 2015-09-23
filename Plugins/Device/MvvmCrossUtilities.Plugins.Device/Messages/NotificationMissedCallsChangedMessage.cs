using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Device.Messages
{
    public class NotificationMissedCallsChangedMessage : NotificationOneWayMessage
    {
        public int Value { get; private set; }

        public NotificationMissedCallsChangedMessage(object sender, int value) : base(sender)
        {
            this.Value = value;
        }
    }
}
