using System.Reflection;
using Android.App;
using Android.OS;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvxExtensions.Platforms.Android.Views;
using Playground.Core.Resources;
using Playground.Core.ViewModels;

namespace Playground.Droid.Views
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme")]
    public class NotificationsView : ActivityBase<NotificationsViewModel>
    {
        //protected override void OnCreate(Bundle savedInstanceState)
        //{
        //    base.OnCreate(savedInstanceState);

        //    SetContentView(Resource.Layout.NotificationsView);

        //    Title = ViewModel.TextSource.GetText(TextResourcesKeys.Label_Page_Title_Notifications);
        //}

        protected override int GetResourceIdFromImageId(string imageId)
        {
            return Resources.GetIdentifier("ic_action_" + imageId, "drawable", ApplicationContext.PackageName);
        }

        protected override FieldInfo GetPageViewFieldInfo(string pageName)
        {
            return typeof(Resource.Layout).GetField(pageName);
        }
    }
}