using System.Collections.Generic;
using System.Reflection;
using Android.Content;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.ViewModels;
using MvxExtensions.Statics;

namespace MvxExtensions.Platforms.Android.Presenters
{
    public class AndroidAppCompatViewPresenter : MvxAppCompatViewPresenter
    {
        public AndroidAppCompatViewPresenter(IEnumerable<Assembly> androidViewAssemblies) : base(androidViewAssemblies)
        {
        }

        protected override Intent CreateIntentForRequest(MvxViewModelRequest request)
        {
            var intent = base.CreateIntentForRequest(request);

            switch (request.ParameterValues?[NavigationModes.NavigationMode])
            {
                case NavigationModes.NavigationModeClearStack:
                    intent.AddFlags(ActivityFlags.ClearTask);
                    break;

                case NavigationModes.NavigationModeRemoveSelf:
                    CurrentActivity?.Finish();
                    break;
            }

            return intent;
        }
    }
}
