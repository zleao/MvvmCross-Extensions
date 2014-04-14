using Android.Content;
using Cirrious.MvvmCross.Droid.Views;

namespace MvvmCrossUtilities.Libraries.Droid.Views
{
    public class ViewsContainer : MvxAndroidViewsContainer
    {
        public ViewsContainer(Context applicationContext)
            : base(applicationContext)
        {
        }

        protected override void AdjustIntentForPresentation(Intent intent, Cirrious.MvvmCross.ViewModels.MvxViewModelRequest request)
        {
            base.AdjustIntentForPresentation(intent, request);

            intent.AddFlags(ActivityFlags.ClearTop);
        }
    }
}