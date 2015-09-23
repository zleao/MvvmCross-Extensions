using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Device.Messages
{
    public class NotificationBatteryChangedMessage : NotificationOneWayMessage
    {
        public BatteryLevelEnum BatteryLevel { get; private set; }

        public NotificationBatteryChangedMessage(object sender, BatteryLevelEnum batteryLevel) : base(sender)
        {
            this.BatteryLevel = batteryLevel;
        }
    }
}
