using Android.App;
using Android.OS;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Playground.Core.Resources;
using Playground.Core.ViewModels;

namespace Playground.Droid.Views
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme")]
    public class MainView : MvxAppCompatActivity<MainViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MainView);

            Title = ViewModel.TextSourceCommon.GetText(TextResourcesKeys.Label_Application_Title);
        }
    }
}