using System;

namespace MvxExtensions.Attributes
{
    /// <summary>
    /// Indicates that the items of the associated collection property, should raise PropertyChanged event
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropagateCollectionChangeAttribute : Attribute
    {
    }        
}
