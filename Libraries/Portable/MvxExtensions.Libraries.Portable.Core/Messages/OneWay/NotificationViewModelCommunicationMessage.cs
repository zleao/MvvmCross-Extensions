using MvxExtensions.Plugins.Notification.Messages.Base;

namespace MvxExtensions.Libraries.Portable.Core.Messages.OneWay
{
    /// <summary>
    /// Message for view model communication notification
    /// </summary>
    public class NotificationViewModelCommunicationMessage : NotificationOneWayMessage
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public object Result
        {
            get { return _result; }
        }
        private readonly object _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationViewModelCommunicationMessage" /> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="result">The result.</param>
        public NotificationViewModelCommunicationMessage(object sender, object result)
            : base(sender)
        {
            _result = result;
        }
    }
}
