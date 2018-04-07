using System;

namespace MvxExtensions.Libraries.Portable.Core.Attributes
{
    /// <summary>
    /// Indicates that the items of the associated collection property, should raise PropertyChanged event
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PropagateCollectionChangeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropagateCollectionChangeAttribute" /> class.
        /// </summary>
        public PropagateCollectionChangeAttribute()
        {
        }
    }        
}
