using Android.Content;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;

namespace MvxExtensions.Libraries.Droid.Core.Views
{
    public class ViewsContainer : MvxAndroidViewsContainer
    {
        public ViewsContainer(Context applicationContext)
            : base(applicationContext)
        {
        }

        protected override void AdjustIntentForPresentation(Intent intent, MvxViewModelRequest request)
        {
            intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.ClearTop);
        }
    }
}