
namespace MvxExtensions.Models
{
    /// <summary>
    /// Tuple for a Key and a Value
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class TupleKeyValue<TKey, TValue> : Model
        where TKey : class
        where TValue : class
    {
        #region Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public TKey Key
        {
            get => _key;
            set
            {
                if (_key != value)
                {
                    _key = value;
                    RaisePropertyChanged(() => Key);
                }
            }
        }
        private TKey _key;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public TValue Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged(() => Value);
                }
            }
        }
        private TValue _value;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TupleKeyValue{TKey, TValue}"/> class.
        /// </summary>
        public TupleKeyValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TupleKeyValue{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public TupleKeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is TupleKeyValue<TKey, TValue> value)
                return Equals(value);

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var keyHash = (Key != null ? Key.GetHashCode() : 0);
            var valueHash = (Value != null ? Value.GetHashCode() : 0);

            return keyHash ^ valueHash;
        }

        #endregion
    }
}
