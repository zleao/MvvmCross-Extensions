using Android.App;
using Android.Widget;
using Android.OS;

namespace MvxExtensions.Samples.AllAround.Droid
{
    [Activity(Label = "MvxExtensions.Samples.AllAround.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }
    }
}

