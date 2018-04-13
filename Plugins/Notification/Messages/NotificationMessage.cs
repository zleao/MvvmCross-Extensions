using System;

namespace MvxExtensions.Plugins.Notification.Messages
{
    /// <summary>
    /// Base class for notification messages that provides weak refrence storage of the sender
    /// </summary>
    public abstract class NotificationMessage : INotificationMessage
    {
        #region Properties

        /// <summary>
        /// Store a WeakReference to the sender just in case anyone is daft enough to
        /// keep the message around and prevent the sender from being collected.
        /// </summary>
        public object Sender
        {
            get
            {
                return (_sender == null) ? null : _sender.Target;
            }
        }
        private readonly WeakReference _sender;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMessage" /> class.
        /// Message will be null
        /// </summary>
        /// <param name="sender">The sender.</param>
        public NotificationMessage(object sender)
        {
            if (sender != null)
                _sender = new WeakReference(sender);
        }

        #endregion
    }
}