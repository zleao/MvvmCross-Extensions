using MvvmCross.Logging;
using MvvmCross.Platforms.Android.Core;
using MvxExtensions.Platforms.Droid.Setup;
using Playground.Core;
using Serilog;

namespace Playground.Droid
{
    public class Setup : AndroidSetup<App>
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