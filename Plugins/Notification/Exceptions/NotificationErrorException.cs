using MvvmCross.Exceptions;
using System;

namespace MvxExtensions.Plugins.Notification.Exceptions
{
    /// <summary>
    /// Represents an exception specific for the notification plugin
    /// </summary>
    public class NotificationErrorException : MvxException
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
