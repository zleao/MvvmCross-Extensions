using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Plugin;

namespace MvxExtensions.Plugins.Logger
{
    [MvxPlugin]
    [Preserve(AllMembers = true)]
    public class Plugin : IMvxPlugin
    {
        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ILogger, Logger>();
        }
    }
}