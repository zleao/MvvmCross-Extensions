﻿using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvxExtensions.Attributes;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Plugins.Notification.Messages;
using MvxExtensions.Plugins.Notification.Messages.OneWay;
using Playground.Core.Resources;
using Playground.Forms.Core.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Playground.Forms.Core.ViewModels
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

        private MenuOption _selectedOption;
        public MenuOption SelectedOption
        {
            get => _selectedOption;
            set => SetProperty(ref _selectedOption, value);
        }

        #endregion

        #region Commands

        public ICommand NavigateCommand { get; }

        #endregion

        #region Constructor

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
                AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_Notifications), typeof(NotificationsViewModel)));
                AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_Storage), typeof(StorageViewModel)));
                
                //Need to have an initialized flag, because despite the fact that this viewmodel is singleton
                //the lifecycle will still be executed every time the view is (re)loaded
                _isInitialized = true;
            }

            return base.Initialize();
        }

        private Task OnNavigateAsync(MenuOption option)
        {
            if (option == null)
            {
                return Task.CompletedTask;
            }

            if (option.ViewModelType == typeof(NotificationsViewModel))
            {
                return NavigationService.Navigate(option.ViewModelType, ViewModelContext);
            }

            return NavigationService.Navigate(option.ViewModelType);
        }

        private Task OnNotificationGenericMessageAsync(NotificationGenericMessage msg)
        {
            return NotificationManager.PublishSuccessNotificationAsync(msg.Message, NotificationModeEnum.MessageBox);
        }

        private void AddOptionIfImplemented(MenuOption option)
        {
            if (HasRegisteredViewFor(option.ViewModelType))
                MenuOptions.Add(option);
        }

        #endregion
    }
}
