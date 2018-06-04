using MvvmCross.Base;
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
            get => _menuOptions;
            set => SetProperty(ref _menuOptions, value);
        }

        #endregion

        #region Commands

        public ICommand NavigateCommand { get; }

        #endregion

        #region Commands

        public MainViewModel(IMvxJsonConverter jsonConverter,
                             INotificationService notificationManager,
                             IMvxLogProvider logProvider,
                             IMvxNavigationService navigationService)
            : base(nameof(MainViewModel), jsonConverter, notificationManager, logProvider, navigationService)
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
