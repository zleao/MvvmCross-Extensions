using System;

namespace MvvmCrossUtilities.Libraries.Portable.Attributes
{
    /// <summary>
    /// Indicates that the associated property/method is dependent on the value of another property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class DependsOnAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the related property
        /// </summary>
        /// <value>
        /// The name of the related property.
        /// </value>
        public string Name
        {
            get { return _name; }
        }
        readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DependsOnAttribute(string name)
        {
            _name = name;
        }
    }        
}
