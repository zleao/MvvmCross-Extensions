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
    public class NotificationsView : MvxAppCompatActivity<NotificationsViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.NotificationsView);

            Title = ViewModel.TextSource.GetText(TextResourcesKeys.Label_Page_Title_Notifications);
        }
    }
}