using MvvmCross;
using MvvmCross.Logging;

namespace MvxExtensions.Plugins.Storage.Utils
{
    internal static class MvxPluginLog
    {
        internal static IMvxLog Instance { get; } = Mvx.IoCProvider.Resolve<IMvxLogProvider>().GetLogFor("MvxPlugin");
    }
}