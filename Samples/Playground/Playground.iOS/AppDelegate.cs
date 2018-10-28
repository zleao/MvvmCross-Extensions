using Foundation;
using MvvmCross.Platforms.Ios.Core;
using MvxExtensions.Platforms.iOS;
using Playground.Core;
using UIKit;

namespace Playground.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : ApplicationDelegate<Setup, App>
    {
        public override UIWindow Window { get; set; }
    }
}

