using MvvmCross.Platform.Platform;

namespace MvxExtensions.Plugins.Notification
{
    internal static class MyTrace
    {
#if DEBUG
        static bool debugEnabled = true;
#else
        static bool debugEnabled = false;
#endif
        public static void Trace(string message)
        {
            if(debugEnabled)
                MvxTrace.Trace(message);
        }

        public static void Warning(string message)
        {
            MvxTrace.Warning(message);
        }
    }
}
