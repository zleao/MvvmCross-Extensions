using System;

namespace MvvmCrossUtilities.Plugins.Notification.Exceptions
{
    public class NotificationErrorException : Cirrious.CrossCore.Exceptions.MvxException
    {
        public NotificationErrorException(Exception ex)
            : base(ex, "NotificationErrorException")
        {
        }
    }
}
