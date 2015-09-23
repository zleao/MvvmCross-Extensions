using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Device.Messages
{
    public class NotificationPcConnectionChangedMessage : NotificationOneWayMessage
    {
        public bool Connected { get; private set; }

        public NotificationPcConnectionChangedMessage(object sender, bool connected) : base(sender)
        {
            this.Connected = connected;
        }
    }
}
