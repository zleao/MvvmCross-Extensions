using MvvmCross;
using MvvmCross.Logging;

namespace MvxExtensions.Plugins.Logger
{
    internal static class MvxPluginLog
    {
        internal static IMvxLog Instance { get; } = Mvx.Resolve<IMvxLogProvider>().GetLogFor("MvxPlugin");
    }
}