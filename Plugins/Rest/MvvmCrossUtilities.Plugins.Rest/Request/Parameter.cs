using System;

namespace MvvmCrossUtilities.Plugins.Rest.Request
{
    /// <summary>
    /// Parameter container for REST requests
    /// </summary>
    public class Parameter
    {
        #region Properties

        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Value of the parameter
        /// </summary>
        public object Value { get; private set; }
        /// <summary>
        /// Type of the parameter
        /// </summary>
        public ParameterType Type { get; private set; }

        /// <summary>
        /// Return a human-readable representation of this parameter
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Value);
        }
        
        #endregion

        #region Constructors

        public Parameter(string name, object value)
            : this(name, value, ParameterType.GetOrPost)
        {
        }

        public Parameter(string name, object value, ParameterType type)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (value == null)
                throw new ArgumentNullException("value");

            Name = name;
            Value = value;
            Type = type;
        }

        #endregion
    }
}
