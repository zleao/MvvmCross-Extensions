using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvxExtensions.Extensions;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Statics;
using Playground.Core.Resources;

namespace Playground.Core.ViewModels
{
    public class NavigationSecondViewModel : BaseViewModel<string>
    {
        #region Properties

        private string _navigationModeDescription;
        public string NavigationModeDescription
        {
            get => _navigationModeDescription;
            set => SetProperty(ref _navigationModeDescription, value);
        }

        #endregion

        #region Commands

        public ICommand NavigateAndClearStackCommand { get; }

        #endregion

        #region Constructor

        public NavigationSecondViewModel(IMvxJsonConverter jsonConverter,
                                         INotificationService notificationManager,
                                         IMvxLogProvider logProvider,
                                         IMvxNavigationService navigationService)
            : base(nameof(NavigationSecondViewModel), jsonConverter, notificationManager, logProvider, navigationService)
        {
            NavigateAndClearStackCommand = new MvxAsyncCommand(OnNavigateAndClearStackAsync);    
        }

        #endregion

        #region Methods

        public override void Prepare(string parameter)
        {
            if (parameter == NavigationModes.NavigationModeRemoveSelf)
            {
                NavigationModeDescription = TextSource.GetText(TextResourcesKeys.Text_NavigateMode_RemoveSelf);
            }
            else
            {
                NavigationModeDescription = TextSource.GetText(TextResourcesKeys.Text_NavigateMode_Normal);
            }
        }

        private Task OnNavigateAndClearStackAsync()
        {
            return NavigationService.NavigateAndClearStack<MainViewModel>();
        }

        #endregion
    }
}
