using System;
using MvvmCrossUtilities.Plugins.Notification.ThreadRunners;

namespace MvvmCrossUtilities.Plugins.Notification.Subscriptions
{
    public enum SubscriptionTypeEnum
    {
        Weak = 0,
        Strong = 1,
    }

    public abstract class Subscription
    {
        #region Constants

        public const string DefaultContext = "DEFAULT";
        
        #endregion

        #region Fields

        private readonly IActionRunner _actionRunner;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public string Context 
        { 
            get { return _context; }
        }
        private readonly string _context;

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id
        {
            get { return _id; }
        }
        private Guid _id;

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsAlive { get; }
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="actionRunner">The action runner.</param>
        /// <param name="context">The context.</param>
        protected Subscription(IActionRunner actionRunner, string context)
        {
            _actionRunner = actionRunner;
            _context = string.IsNullOrEmpty(context) ? DefaultContext : context;
            _id = Guid.NewGuid();
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Calls the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        protected void Call(Action action)
        {
            _actionRunner.Run(action);
        }

        #endregion
    }
}