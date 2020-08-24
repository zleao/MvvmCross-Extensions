using Android.App;
using Android.Content.PM;
using MvvmCross.Platforms.Android.Views;

namespace Playground.Droid
{
    [Activity(
        Label = "Playground.Droid"
        , MainLauncher = true
        , Theme = "@style/AppTheme.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }
    }
}