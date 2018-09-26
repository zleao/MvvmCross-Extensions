using MvvmCross.Logging;
using Playground.Core;
using Serilog;
using MvxExtensions.Platforms.Android.Setup;

namespace Playground.Droid
{
    public class Setup : AndroidAppCompatSetup<App>
    {
        public override MvxLogProviderType GetDefaultLogProviderType()
            => MvxLogProviderType.Serilog;

        protected override IMvxLogProvider CreateLogProvider()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.AndroidLog()
                .CreateLogger();
            return base.CreateLogProvider();
        }
    }
}