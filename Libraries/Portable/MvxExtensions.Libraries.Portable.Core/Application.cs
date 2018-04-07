using MvvmCross.Core.ViewModels;

namespace MvxExtensions.Libraries.Portable.Core
{
    /// <summary>
    /// Base application class inherited by <see cref="MvxApplication"/>
    /// </summary>
    public abstract class Application : MvxApplication
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            RegisterModels();
            RegisterServices();
        }

        /// <summary>
        /// Entry point for models registration
        /// </summary>
        protected virtual void RegisterModels()
        {
        }

        /// <summary>
        /// Entry point for services registration
        /// </summary>
        protected virtual void RegisterServices()
        {
        }
    }
}
