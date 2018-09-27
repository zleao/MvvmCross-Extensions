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
using MvxExtensions.Attributes;
using MvxExtensions.Plugins.Notification.Messages;
using MvxExtensions.Plugins.Notification.Messages.OneWay;

namespace Playground.Core.ViewModels
{
    [SingletonViewModel]
    public class MainViewModel : BaseViewModel
    {
        #region Fields

        private bool _isInitialized;

        #endregion
        
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

        #region Subscriptions

        protected override void SubscribeLongRunningMessageEvents()
        {
            base.SubscribeLongRunningMessageEvents();

            SubscribeLongRunningEvent<NotificationGenericMessage>(OnNotificationGenericMessageAsync, nameof(OnNotificationGenericMessageAsync), context: ViewModelContext);
        }

        #endregion

        #region Methods

        public override Task Initialize()
        {
            if (!_isInitialized)
            {
                AddMenuOption(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_Notifications), typeof(NotificationsViewModel)));
                AddMenuOption(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_Navigation), typeof(NavigationViewModel)));
                _isInitialized = true;
            }

            return base.Initialize();
        }

        private Task OnNavigateAsync(MenuOption option)
        {
            if (option.ViewModelType == typeof(NotificationsViewModel))
            {
                return NavigationService.Navigate(option.ViewModelType, ViewModelContext);
            }

            return NavigationService.Navigate(option.ViewModelType);
        }

        private void AddMenuOption(MenuOption option)
        {
            MenuOptions.Add(option);
        }

        private Task OnNotificationGenericMessageAsync(NotificationGenericMessage msg)
        {
            return NotificationManager.PublishSuccessNotificationAsync(msg.Message, NotificationModeEnum.MessageBox);
        }

        #endregion
    }
}
