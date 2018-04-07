﻿using System;

namespace MvxExtensions.Libraries.Portable.Core.Attributes
{
    /// <summary>
    /// Used to bind the current property/method to the PropertyChanged event of another property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class DependsOnAttribute : Attribute
    {
        /// <summary>
        /// Name of the property to listen to
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        readonly string _name;

        /// <summary>
        /// Indicates if the dependency is conditional, in order to be able to cancel propagations
        /// </summary>
        public bool IsConditional
        {
            get { return _isConditional; }
        }
        readonly bool _isConditional;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnAttribute" /> class.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="isConditional">Indicates if the dependency is conditional, in order to be able to cancel propagations</param>
        public DependsOnAttribute(string name, bool isConditional = false)
        {
            _name = name;
            _isConditional = isConditional;
        }
    }
}
