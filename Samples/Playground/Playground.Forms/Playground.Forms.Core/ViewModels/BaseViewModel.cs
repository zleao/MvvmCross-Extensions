using MvvmCross.Base;
using MvvmCross.Localization;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.ViewModels;

namespace Playground.Forms.Core.ViewModels
{
    public abstract class BaseViewModel : ViewModel
    {
        #region Constants

        private const string ProjectBaseName = "Playground.Forms.Core";

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
                   logProvider,
                   navigationService)
        {
        }

        #endregion
    }

    public abstract class BaseViewModel<TParameter> : ViewModel<TParameter>
    {
        #region Constants

        private const string ProjectBaseName = "Playground.Forms.Core";

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
                   logProvider,
                   navigationService)
        {
        }

        #endregion
    }
}
