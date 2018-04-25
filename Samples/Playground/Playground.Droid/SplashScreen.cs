using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Support.V7.AppCompat;

namespace Playground.Droid
{
     [Activity(
        Label = "Playground.Droid"
        , MainLauncher = true
        , Theme = "@style/AppTheme.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenAppCompatActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }
    }
}