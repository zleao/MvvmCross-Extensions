using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvxExtensions.Extensions;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Statics;

namespace Playground.Core.ViewModels
{
    public class NavigationViewModel : BaseViewModel
    {
        #region Commands

        public ICommand NavigateAndRemoveSelfCommand { get; }
        public ICommand NavigateCommand { get; }

        #endregion

        #region Constructor

        public NavigationViewModel(IMvxJsonConverter jsonConverter,
                                   INotificationService notificationManager,
                                   IMvxLogProvider logProvider,
                                   IMvxNavigationService navigationService)
            : base(nameof(NavigationViewModel), jsonConverter, notificationManager, logProvider, navigationService)
        {
            NavigateAndRemoveSelfCommand = new MvxAsyncCommand(OnNavigateAndRemoveSelfAsync);    
            NavigateCommand = new MvxAsyncCommand(OnNavigateAsync);
        }

        #endregion

        #region Methods

        private Task OnNavigateAndRemoveSelfAsync()
        {
            return NavigationService.NavigateAndRemoveSelf<NavigationSecondViewModel, string>(NavigationModes.NavigationModeRemoveSelf);
        }

        private Task OnNavigateAsync()
        {
            return NavigationService.Navigate<NavigationSecondViewModel, string>(null);
        }

        #endregion

    }
}
