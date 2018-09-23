using MvvmCross.Base;
using MvvmCross.Localization;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.ViewModels;

namespace Playground.Core.ViewModels
{
    public abstract class BaseViewModel : ViewModel
    {
        #region Constants

        private static readonly string ProjectBaseName = "Playground.Core";

        #endregion

        #region Fields
        

        #endregion

        #region Properties

        protected IMvxNavigationService NavigationService {get;}

        #endregion

        #region Constructor

        protected BaseViewModel(string textSourceFileName, 
                                IMvxJsonConverter jsonConverter, 
                                INotificationService notificationManager, 
                                IMvxLogProvider logProvider, 
                                IMvxNavigationService navigationService)
            : base(new MvxLanguageBinder(ProjectBaseName, textSourceFileName), 
                   new MvxLanguageBinder(ProjectBaseName, "Common"), 
                   jsonConverter, 
                   notificationManager, 
                   logProvider)
        {
            NavigationService = navigationService;
        }

        #endregion
    }

    public abstract class BaseViewModel<TParameter> : ViewModel<TParameter>
    {
        #region Constants

        private static readonly string ProjectBaseName = "Playground.Core";

        #endregion

        #region Fields
        

        #endregion

        #region Properties

        protected IMvxNavigationService NavigationService {get;}

        #endregion

        #region Constructor

        protected BaseViewModel(string textSourceFileName, 
            IMvxJsonConverter jsonConverter, 
            INotificationService notificationManager, 
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService)
            : base(new MvxLanguageBinder(ProjectBaseName, textSourceFileName), 
                new MvxLanguageBinder(ProjectBaseName, "Common"), 
                jsonConverter, 
                notificationManager, 
                logProvider)
        {
            NavigationService = navigationService;
        }

        #endregion
    }
}
