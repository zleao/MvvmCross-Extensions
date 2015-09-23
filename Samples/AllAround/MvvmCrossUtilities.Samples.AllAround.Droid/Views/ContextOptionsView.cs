using Android.App;
using Android.Content.PM;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels;
using MvvmCrossUtilities.Samples.AllAround.Droid.Views.Base;

namespace MvvmCrossUtilities.Samples.AllAround.Droid.Views
{
    [Activity(Icon = "@drawable/Icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ContextOptionsView : BaseView<ContextOptionsViewModel>
    {
    }
}