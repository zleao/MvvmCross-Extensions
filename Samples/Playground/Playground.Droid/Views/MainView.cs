using System.Reflection;
using Android.App;
using Android.OS;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvxExtensions.Platforms.Android.Views;
using Playground.Core.ViewModels;

namespace Playground.Droid.Views
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme")]
    public class MainView : AppCompatActivityBase<MainViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Title = ViewModel.PageTitle;
        }

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