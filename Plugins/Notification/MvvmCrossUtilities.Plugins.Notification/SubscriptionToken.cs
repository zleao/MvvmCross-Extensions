using System;

namespace MvvmCrossUtilities.Plugins.Notification
{
    public class SubscriptionToken
    {
        #region Properties

        public Type MessageType
        {
            get { return _messageType; }
        }
        private readonly Type _messageType;

        public string Context
        {
            get { return _context; }
        }
        private readonly string _context;

        public Guid Id 
        { 
            get { return _id; } 
        }
        private readonly Guid _id;

        public object[] DependentObjects
        {
            get { return _dependentObjects; }
        }
        private readonly object[] _dependentObjects;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionToken"/> class.
        /// </summary>
        /// <param name="MessageType">Type of the message.</param>
        /// <param name="context">The context.</param>
        /// <param name="id">The id.</param>
        /// <param name="dependentObjects">The dependent objects.</param>
        public SubscriptionToken(Type MessageType, string context, Guid id, params object[] dependentObjects)
        {
            _messageType = MessageType;
            _context = context;
            _id = id;
            _dependentObjects = dependentObjects;
        }
        
        #endregion
    }
}