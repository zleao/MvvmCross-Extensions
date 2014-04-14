using Android.App;
using Android.Content.PM;
using Android.Views;
using MvvmCrossUtilities.Samples.AllAround.Core.Rest;
using MvvmCrossUtilities.Samples.AllAround.Droid.Views.Base;

namespace MvvmCrossUtilities.Samples.AllAround.Droid.Views
{
    [Activity(Icon = "@drawable/Icon", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateHidden)]
    public class ArticlesView : BaseView<ArticlesViewModel>
    {
    }
}