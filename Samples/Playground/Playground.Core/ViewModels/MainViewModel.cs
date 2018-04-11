using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Playground.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly IMvxLogProvider _logProvider;
        private readonly IMvxViewModelLoader _mvxViewModelLoader;

        public MainViewModel(IMvxNavigationService navigationService, IMvxLogProvider logProvider, IMvxViewModelLoader mvxViewModelLoader)
        {
            _navigationService = navigationService;
            _logProvider = logProvider;
            _mvxViewModelLoader = mvxViewModelLoader;
        }


    }
}
