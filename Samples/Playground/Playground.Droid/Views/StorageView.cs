using Android.App;
using Android.OS;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Playground.Core.ViewModels;

namespace Playground.Droid.Views
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme")]
    public class StorageView : BaseAppCompatView<StorageViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Title = ViewModel.PageTitle;
        }
    }
}