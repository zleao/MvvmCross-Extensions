using MvvmCross.Logging;
using MvxExtensions.Platforms.Droid.Support.V7.Setup;
using Playground.Core;
using Serilog;

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