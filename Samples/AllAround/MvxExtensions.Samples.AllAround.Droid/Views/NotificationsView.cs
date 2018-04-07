using Android.App;
using Android.Content.PM;
using MvxExtensions.Samples.AllAround.Core.ViewModels;
using MvxExtensions.Samples.AllAround.Droid.Views.Base;

namespace MvxExtensions.Samples.AllAround.Droid.Views
{
    [Activity(Icon = "@drawable/Icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class NotificationsView : SimpleMenuBaseView<NotificationsViewModel>
    {
    }
}