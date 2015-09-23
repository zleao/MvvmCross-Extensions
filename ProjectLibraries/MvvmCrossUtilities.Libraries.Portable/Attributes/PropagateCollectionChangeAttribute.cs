using System;

namespace MvvmCrossUtilities.Libraries.Portable.Attributes
{
    /// <summary>
    /// Indicates that the items of the associated collection property, should raise property changes
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
