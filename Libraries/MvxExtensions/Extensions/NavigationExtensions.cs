using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxExtensions.Statics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MvxExtensions.Extensions
{
    /// <summary>
    /// Extensions for the IMvxNavigationService
    /// </summary>
    public static class NavigationExtensions
    {
        /// <summary>
        /// Not Supported in Xamarin.Forms Projects!
        /// Navigates to a ViewModel Type.
        /// Removes the current viewmodel/view from the stack
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static Task NavigateAndRemoveSelf(this IMvxNavigationService navigationService, Type viewModelType, IMvxBundle presentationBundle = null, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (presentationBundle == null)
            {
                presentationBundle = new MvxBundle();
            }

            presentationBundle.Data.SafeAddOrUpdate(NavigationModes.NavigationMode, NavigationModes.NavigationModeRemoveSelf);

            return navigationService?.Navigate(viewModelType, presentationBundle, cancellationToken);
        }

        /// <summary>
        /// Not Supported in Xamarin.Forms Projects!
        /// Navigates to a ViewModel Type and passes TParameter
        /// Removes the current viewmodel/view from the stack
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        public static Task NavigateAndRemoveSelf<TParameter>(this IMvxNavigationService navigationService, Type viewModelType, TParameter param, IMvxBundle presentationBundle = null)
        {
            if (presentationBundle == null)
            {
                presentationBundle = new MvxBundle();
            }

            presentationBundle.Data.SafeAddOrUpdate(NavigationModes.NavigationMode, NavigationModes.NavigationModeRemoveSelf);

            return navigationService?.Navigate(viewModelType, param, presentationBundle);
        }

        /// <summary>
        /// Not Supported in Xamarin.Forms Projects!
        /// Navigates to a ViewModel Type.
        /// Removes the current viewmodel/view from the stack
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        public static Task NavigateAndRemoveSelf<TViewModel>(this IMvxNavigationService navigationService, IMvxBundle presentationBundle = null) where TViewModel : IMvxViewModel
        {
            if (presentationBundle == null)
            {
                presentationBundle = new MvxBundle();
            }

            presentationBundle.Data.SafeAddOrUpdate(NavigationModes.NavigationMode, NavigationModes.NavigationModeRemoveSelf);

            return navigationService?.Navigate<TViewModel>(presentationBundle);
        }

        /// <summary>
        /// Not Supported in Xamarin.Forms Projects!
        /// Navigates to a ViewModel Type and passes TParameter
        /// Removes the current viewmodel/view from the stack
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        public static Task NavigateAndRemoveSelf<TViewModel, TParameter>(this IMvxNavigationService navigationService, TParameter param, IMvxBundle presentationBundle = null) where TViewModel : IMvxViewModel<TParameter>
        {
            if (presentationBundle == null)
            {
                presentationBundle = new MvxBundle();
            }

            presentationBundle.Data.SafeAddOrUpdate(NavigationModes.NavigationMode, NavigationModes.NavigationModeRemoveSelf);

            return navigationService?.Navigate<TViewModel, TParameter>(param, presentationBundle);
        }


        /// <summary>
        /// Not Supported in Xamarin.Forms Projects!
        /// Navigates to a ViewModel Type.
        /// Clears the current view stack
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static Task NavigateAndClearStack(this IMvxNavigationService navigationService, Type viewModelType, IMvxBundle presentationBundle = null, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (presentationBundle == null)
            {
                presentationBundle = new MvxBundle();
            }

            presentationBundle.Data.SafeAddOrUpdate(NavigationModes.NavigationMode, NavigationModes.NavigationModeClearStack);

            return navigationService?.Navigate(viewModelType, presentationBundle, cancellationToken);
        }

        /// <summary>
        /// Not Supported in Xamarin.Forms Projects!
        /// Navigates to a ViewModel Type and passes TParameter
        /// Clears the current view stack
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        public static Task NavigateAndClearStack<TParameter>(this IMvxNavigationService navigationService, Type viewModelType, TParameter param, IMvxBundle presentationBundle = null)
        {
            if (presentationBundle == null)
            {
                presentationBundle = new MvxBundle();
            }

            presentationBundle.Data.SafeAddOrUpdate(NavigationModes.NavigationMode, NavigationModes.NavigationModeClearStack);

            return navigationService?.Navigate(viewModelType, param, presentationBundle);
        }

        /// <summary>
        /// Not Supported in Xamarin.Forms Projects!
        /// Navigates to a ViewModel Type.
        /// Clears the current view stack
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        public static Task NavigateAndClearStack<TViewModel>(this IMvxNavigationService navigationService, IMvxBundle presentationBundle = null) where TViewModel : IMvxViewModel
        {
            if (presentationBundle == null)
            {
                presentationBundle = new MvxBundle();
            }

            presentationBundle.Data.SafeAddOrUpdate(NavigationModes.NavigationMode, NavigationModes.NavigationModeClearStack);

            return navigationService?.Navigate<TViewModel>(presentationBundle);
        }

        /// <summary>
        /// Not Supported in Xamarin.Forms Projects!
        /// Navigates to a ViewModel Type and passes TParameter
        /// Clears the current view stack
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        public static Task NavigateAndClearStack<TViewModel, TParameter>(this IMvxNavigationService navigationService, TParameter param, IMvxBundle presentationBundle = null) where TViewModel : IMvxViewModel<TParameter>
        {
            if (presentationBundle == null)
            {
                presentationBundle = new MvxBundle();
            }

            presentationBundle.Data.SafeAddOrUpdate(NavigationModes.NavigationMode, NavigationModes.NavigationModeClearStack);

            return navigationService?.Navigate<TViewModel, TParameter>(param, presentationBundle);
        }

    }
}
