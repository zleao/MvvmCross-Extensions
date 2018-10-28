using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Playground.Core.ViewModels;

namespace Playground.Droid.Views
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme")]
    public class MainView : BaseAppCompatView<MainViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Title = ViewModel.PageTitle;
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (BaseContext.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Denied)
            {
                 RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, 1000);
            } 
        }
    }
}