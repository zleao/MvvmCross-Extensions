using System;

namespace MvvmCrossUtilities.Libraries.Portable.Attributes
{
    /// <summary>
    /// Simple attribute class for storing String Values
    /// </summary>
    public sealed class StringValueAttribute : Attribute
    {

        /// <summary>
        /// Creates a new <see cref="StringValueAttribute"/> instance.
        /// </summary>
        /// <param name="value">Value.</param>
        public StringValueAttribute(string value) : this("", value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringValueAttribute"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="value">The value.</param>
        public StringValueAttribute(string code, string value)
        {
            _code = code;
            _value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value></value>
        public string Value
        {
            get { return _value; }
        }
        private string _value;

        /// <summary>
        /// Gets the code.
        /// </summary>
        public string Code
        {
            get { return _code; }
        }
        private string _code;
    }
}
