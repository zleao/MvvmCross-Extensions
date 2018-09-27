using System;
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
            try
            {
                var navigationMode = request.PresentationValues?[NavigationModes.NavigationMode];
            
                if (navigationMode == NavigationModes.NavigationModeClearStack)
                {
                    intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
                }
                else if (navigationMode == NavigationModes.NavigationModeRemoveSelf)
                {
                    CurrentActivity?.Finish();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return intent;
        }
    }
}
