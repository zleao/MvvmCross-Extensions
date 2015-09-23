using MvvmCrossUtilities.Plugins.Notification.Core.Async.ThreadRunners;
using System;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Plugins.Notification.Core.Async.Subscriptions
{
    /// <summary>
    /// Asynchronous notification subscription
    /// </summary>
    public abstract class AsyncSubscription : ISubscription
    {
        #region Constants

        /// <summary>
        /// The default context
        /// </summary>
        public const string DefaultContext = "DEFAULT";

        #endregion

        #region Fields

        private readonly IAsyncActionRunner _asyncActionRunner;

        #endregion

        #region Properties

        /// <summary>
        /// Context of the notification.
        /// </summary>
        public string Context
        {
            get { return _context; }
        }
        private readonly string _context;

        /// <summary>
        /// Unique identificer of the subscription.
        /// </summary>
        public Guid Id
        {
            get { return _id; }
        }
        private Guid _id;

        /// <summary>
        /// Indcates if the subscriber is still connected to this subscription instance
        /// </summary>
        public abstract bool IsAlive { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncSubscription"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="context">The context.</param>
        protected AsyncSubscription(IAsyncActionRunner asyncActionRunner, string context)
        {
            _asyncActionRunner = asyncActionRunner;
            _context = string.IsNullOrEmpty(context) ? DefaultContext : context;
            _id = Guid.NewGuid();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the specified asynchronous action.
        /// </summary>
        /// <param name="actionAsync">The action asynchronous.</param>
        /// <returns></returns>
        protected Task CallAsync(Func<Task> actionAsync)
        {
            return _asyncActionRunner.RunAsync(actionAsync);
        }

        #endregion
    }

    /// <summary>
    /// Async subscription with a result
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public abstract class AsyncSubscription<TResult> : ISubscription
    {
        #region Constants

        /// <summary>
        /// The default context
        /// </summary>
        public const string DefaultContext = "DEFAULT";
        
        #endregion

        #region Fields

        private readonly IAsyncActionRunner<TResult> _asyncActionRunner;

        #endregion

        #region Properties

        /// <summary>
        /// Context of the notification.
        /// </summary>
        public string Context 
        { 
            get { return _context; }
        }
        private readonly string _context;

        /// <summary>
        /// Unique identificer of the subscription.
        /// </summary>
        public Guid Id
        {
            get { return _id; }
        }
        private Guid _id;

        /// <summary>
        /// Indcates if the subscriber is still connected to this subscription instance
        /// </summary>
        public abstract bool IsAlive { get; }
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncSubscription{TResult}"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="context">The context.</param>
        protected AsyncSubscription(IAsyncActionRunner<TResult> asyncActionRunner, string context)
        {
            _asyncActionRunner = asyncActionRunner;
            _context = string.IsNullOrEmpty(context) ? DefaultContext : context;
            _id = Guid.NewGuid();
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Executes the specified asynchronous action.
        /// </summary>
        /// <param name="actionAsync">The asynchronous action.</param>
        /// <returns></returns>
        protected Task<TResult> CallAsync(Func<Task<TResult>> actionAsync)
        {
            return _asyncActionRunner.RunAsync(actionAsync);
        }

        #endregion
    }
}