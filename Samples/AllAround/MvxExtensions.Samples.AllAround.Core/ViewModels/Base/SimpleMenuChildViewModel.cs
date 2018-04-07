using MvvmCross.Localization;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvxExtensions.Libraries.Portable.Core.ViewModels.SimpleMenu;
using MvxExtensions.Plugins.Notification;

namespace MvxExtensions.Samples.AllAround.Core.ViewModels.Base
{
    public abstract class AllAroundChildViewModel<TParentViewModel> : SimpleMenuChildViewModel<TParentViewModel>
        where TParentViewModel : SimpleMenuBaseViewModel
    {
        #region Constructor

        protected AllAroundChildViewModel(TParentViewModel parent)
            : base(Mvx.Resolve<IMvxLanguageBinder>(),
                   Mvx.Resolve<IMvxJsonConverter>(),
                   Mvx.Resolve<INotificationService>(),
                   parent)
        {
        }

        #endregion
    }
}
