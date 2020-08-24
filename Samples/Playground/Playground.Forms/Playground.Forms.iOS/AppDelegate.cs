using Foundation;
using MvvmCross.Forms.Platforms.Ios.Core;

namespace Playground.Forms.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : MvxFormsApplicationDelegate<MvxFormsIosSetup<Core.App, App>, Core.App, App>
    {
    }
}
