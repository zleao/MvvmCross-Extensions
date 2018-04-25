using MvvmCross.Commands;
using MvvmCross.Localization;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Playground.Core.Models;
using Playground.Core.Resources;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Playground.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Fields

        private readonly IMvxNavigationService _navigationService;
        private readonly IMvxLogProvider _logProvider;
        private readonly IMvxViewModelLoader _mvxViewModelLoader;

        #endregion

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

        public MainViewModel(IMvxNavigationService navigationService,
                             IMvxLogProvider logProvider,
                             IMvxViewModelLoader mvxViewModelLoader)
        {
            _navigationService = navigationService;
            _logProvider = logProvider;
            _mvxViewModelLoader = mvxViewModelLoader;

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
            await _navigationService.Navigate(option.ViewModelType);
        }

        private void AddMenuOption(MenuOption option)
        {
            MenuOptions.Add(option);
        }

        #endregion
    }
}
