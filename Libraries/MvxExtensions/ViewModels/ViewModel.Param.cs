using MvvmCross.Base;
using MvvmCross.Localization;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxExtensions.Plugins.Notification;

namespace MvxExtensions.ViewModels
{
    /// <summary>
    /// Base viewmodel with input, built on top of <see cref="CoreViewModel"/>
    /// </summary>
    public abstract class ViewModel<TParameter> : ViewModel, IMvxViewModel<TParameter>
    {
        protected ViewModel(IMvxLanguageBinder textSource,
                            IMvxLanguageBinder textSourceCommon,
                            IMvxJsonConverter jsonConverter,
                            INotificationService notificationManager,
                            IMvxLogProvider logProvider,
                            IMvxNavigationService navigationService)
            : base(textSource, textSourceCommon, jsonConverter, notificationManager, logProvider, navigationService)
        {
        }

        public abstract void Prepare(TParameter parameter);
    }
}