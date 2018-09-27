using System;
using MvxExtensions.ViewModels;

namespace MvxExtensions.Attributes
{
    /// <summary>
    /// Attribute used to mark viewmodels that are to be used has singletons,
    /// forcing the viewmodel locator to cache this viewmodel
    /// You must use the <see cref="SingletonViewModelLocator" /> for the cache to work properly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class SingletonViewModelAttribute : Attribute
    {
    }
}
