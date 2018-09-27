using MvxExtensions.Plugins.Notification.Core;

namespace MvxExtensions.Models
{
    /// <summary>
    ///  Token for one-way long running subscriptions
    /// </summary>
    public class LongRunningSubscriptionToken
    {
        #region Properties

        /// <summary>
        /// Associated subscription token.
        /// </summary>
        public SubscriptionToken Token { get; }

        /// <summary>
        /// Gets the delivery action.
        /// </summary>
        public object AsyncDeliveryAction { get; }

        /// <summary>
        /// Gets the name of the asynchronous delivery action.
        /// </summary>
        /// <value>
        /// The name of the asynchronous delivery action.
        /// </value>
        public string AsyncDeliveryActionName { get; }

        /// <summary>
        /// Gets a value indicating whether to unsubscribe on arrival.
        /// </summary>
        /// <value>
        /// <c>true</c> if unsubscribe on arrival; otherwise, <c>false</c>.
        /// </value>
        public bool UnsubscribeOnArrival { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LongRunningSubscriptionToken" /> class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="asyncDeliveryActionName">Name of the asynchronous delivery action.</param>
        /// <param name="unsubscribeOnArrival">if set to <c>true</c> [unsubscribe on arrival].</param>
        public LongRunningSubscriptionToken(SubscriptionToken token, object asyncDeliveryAction, string asyncDeliveryActionName,  bool unsubscribeOnArrival)
        {
            Token = token;
            AsyncDeliveryAction = asyncDeliveryAction;
            AsyncDeliveryActionName = asyncDeliveryActionName;
            UnsubscribeOnArrival = unsubscribeOnArrival;
        }

        #endregion
    }
}
