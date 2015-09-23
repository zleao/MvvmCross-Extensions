using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Device.Messages
{
    public class NotificationNetworkSignalChangedMessage : NotificationOneWayMessage
    {
        public int SignalStrength { get; private set; }

        public NotificationNetworkSignalChangedMessage(object sender, int signalStrength) : base(sender)
        {
            this.SignalStrength = signalStrength;
        }
    }
}
