using System;

namespace MvxExtensions.Attributes
{
    /// <summary>
    /// Simple attribute class for storing String Values
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
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
            Code = code;
            Value = value;            
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value></value>
        public string Value { get; }

        /// <summary>
        /// Gets the code.
        /// </summary>
        public string Code { get; }
    }        
}
