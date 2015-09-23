using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Device.Messages
{
    public class NotificationNetworkTypeChangedMessage : NotificationOneWayMessage
    {
        public NetworkTypeEnum NetworkType { get; private set; }

        public NotificationNetworkTypeChangedMessage(object sender, NetworkTypeEnum networkType) : base(sender)
        {
            this.NetworkType = networkType;
        }
    }
}
