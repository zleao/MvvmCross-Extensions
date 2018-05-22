using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvxExtensions.Plugins.Notification;
using Playground.Core.Models;
using Playground.Core.Resources;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Playground.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties

        private ObservableCollection<MenuOption> _menuOptions = new ObservableCollection<MenuOption>();
        public ObservableCollection<MenuOption> MenuOptions
        {
            get { return _menuOptions; }
            set { SetProperty(ref _menuOptions, value); }
        }

        #endregion

        #region Commands

        public ICommand NavigateCommand { get; }

        #endregion

        #region Commands

        public MainViewModel(IMvxNavigationService navigationService, IMvxLogProvider logProvider, INotificationService notificationManager)
            : base(navigationService, logProvider, notificationManager)
        {
            NavigateCommand = new MvxAsyncCommand<MenuOption>(OnNavigateAsync);
        }

        #endregion

        #region Methods

        public override Task Initialize()
        {
            AddMenuOption(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_Notifications), typeof(NotificationsViewModel)));

            return base.Initialize();
        }

        private async Task OnNavigateAsync(MenuOption option)
        {
            await NavigationService.Navigate(option.ViewModelType);
        }

        private void AddMenuOption(MenuOption option)
        {
            MenuOptions.Add(option);
        }

        #endregion
    }
}
