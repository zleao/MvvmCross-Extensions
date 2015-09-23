using System;

namespace MvvmCrossUtilities.Plugins.Notification.Exceptions
{
    /// <summary>
    /// Represents an exception specific for the notification plugin
    /// </summary>
    public class NotificationErrorException : Cirrious.CrossCore.Exceptions.MvxException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationErrorException"/> class.
        /// </summary>
        /// <param name="ex">The inner exception</param>
        public NotificationErrorException(Exception ex)
            : base(ex, "NotificationErrorException")
        {
        }
    }
}
