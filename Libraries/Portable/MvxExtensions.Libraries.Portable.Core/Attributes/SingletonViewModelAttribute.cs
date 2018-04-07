using MvxExtensions.Libraries.Portable.Core.ViewModels.Utilities;
using System;

namespace MvxExtensions.Libraries.Portable.Core.Attributes
{
    /// <summary>
    /// Attribute used to mark viewmodels that are to be used has singletons,
    /// forcing the viewmodel locator to cache this viewmodel
    /// You must use the <see cref="SingletonViewModelLocator" /> for the cache to work properly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class SingletonViewModelAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonViewModelAttribute" /> class.
        /// </summary>
        public SingletonViewModelAttribute()
        {
        }
    }
}
