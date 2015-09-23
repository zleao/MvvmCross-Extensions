namespace MvvmCrossUtilities.Libraries.Portable.Messages.OneWay
{
    /// <summary>
    /// Message for data updated notification
    /// </summary>
    public class NotificationDataUpdatedMessage : NotificationViewModelCommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDataUpdatedMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public NotificationDataUpdatedMessage(object sender)
            : base(sender, null)
        {
        }
    }
}
