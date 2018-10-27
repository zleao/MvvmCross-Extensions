using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.ViewModels;
using MvxExtensions.Statics;
using System;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace MvxExtensions.Platforms.iOS.Presenters
{
    public class iOSViewPresenter : MvxIosViewPresenter
    {
        public iOSViewPresenter(IUIApplicationDelegate applicationDelegate, UIWindow window) 
            : base(applicationDelegate, window)
        {
        }

        protected override async Task<bool> ShowChildViewController(UIViewController viewController, MvxChildPresentationAttribute attribute, MvxViewModelRequest request)
        {
            if(! await base.ShowChildViewController(viewController, attribute, request))
            {
                return false;
            }

            try
            {
                if (MasterNavigationController != null)
                {
                    var navigationMode = request.PresentationValues?[NavigationModes.NavigationMode];

                    if (navigationMode == NavigationModes.NavigationModeClearStack)
                    {
                        MasterNavigationController.ViewControllers = new UIViewController[] { viewController };
                    }
                    else if (navigationMode == NavigationModes.NavigationModeRemoveSelf)
                    {
                        MasterNavigationController.ViewControllers = MasterNavigationController.ViewControllers.Take(MasterNavigationController.ViewControllers.Count() - 1).ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return true;
        }

    }
}
