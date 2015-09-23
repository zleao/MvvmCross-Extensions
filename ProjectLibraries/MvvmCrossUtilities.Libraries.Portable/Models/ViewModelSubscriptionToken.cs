using MvvmCrossUtilities.Plugins.Notification.Core;

namespace MvvmCrossUtilities.Libraries.Portable.Models
{
    /// <summary>
    /// ViewModelSubscriptionToken
    /// </summary>
    public class ViewModelSubscriptionToken
    {
        #region Properties

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public SubscriptionToken Token { get; private set; }

        /// <summary>
        /// Gets the delivery action.
        /// </summary>
        /// <value>
        /// The delivery action.
        /// </value>
        public object DeliveryAction { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to unsubscribe on arrival.
        /// </summary>
        /// <value>
        /// <c>true</c> if unsubscribe on arrival; otherwise, <c>false</c>.
        /// </value>
        public bool UnsubscribeOnArrival { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelSubscriptionToken"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="deliveryAction">The delivery action.</param>
        /// <param name="unsubscribeOnArrival">if set to <c>true</c> [unsubscribe on arrival].</param>
        public ViewModelSubscriptionToken(SubscriptionToken token, object deliveryAction, bool unsubscribeOnArrival)
        {
            Token = token;
            DeliveryAction = deliveryAction;
            UnsubscribeOnArrival = unsubscribeOnArrival;
        }

        #endregion
    }
}
