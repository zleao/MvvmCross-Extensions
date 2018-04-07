using Android.Content;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;

namespace MvxExtensions.Libraries.Droid.Core.Views
{
    public class AndroidViewPresenter : MvxAndroidViewPresenter, IAndroidViewPresenter
    {
        private bool _clearBackStack;

        public override void Show(MvxViewModelRequest request)
        {
            _clearBackStack = false;

            base.Show(request);
        }

        public void Show(MvxViewModelRequest request, bool removeCurrentViewFromStack, bool clearBackStack)
        {
            _clearBackStack = clearBackStack;

            base.Show(request);

            if (!clearBackStack && removeCurrentViewFromStack && Activity != null)
                    Activity.Finish();
        }

        protected override Intent CreateIntentForRequest(MvxViewModelRequest request)
        {
            var requestTranslator = Mvx.Resolve<IMvxAndroidViewModelRequestTranslator>();
            var intent = requestTranslator.GetIntentFor(request);

            if (_clearBackStack)
                intent.AddFlags(ActivityFlags.ClearTask);

            return intent;
        }
    }
}