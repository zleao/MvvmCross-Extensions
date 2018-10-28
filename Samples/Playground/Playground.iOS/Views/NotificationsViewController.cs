using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvxExtensions.Platforms.iOS.Views;
using Playground.Core.Resources;
using Playground.Core.ViewModels;
using System;

namespace Playground.iOS
{
    [MvxFromStoryboard("Main")]
    [MvxChildPresentation]
    public partial class NotificationsViewController : ViewControllerBase<NotificationsViewModel>
    {
        public NotificationsViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel?.PageTitle;

            var set = this.CreateBindingSet<NotificationsViewController, NotificationsViewModel>();

            set.Bind(ErrorNotifBtn).For(nameof(ErrorNotifBtn.Title)).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(TextResourcesKeys.Label_Button_ErrorNotification);
            set.Bind(ErrorNotifBtn).To(vm => vm.ErrorNotificationCommand);

            set.Bind(QuestionNotifBtn).For(nameof(QuestionNotifBtn.Title)).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(TextResourcesKeys.Label_Button_QuestionNotification);
            set.Bind(QuestionNotifBtn).To(vm => vm.QuestionNotificationCommand);

            set.Bind(DelayedNotifBtn).For(nameof(DelayedNotifBtn.Title)).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(TextResourcesKeys.Label_Button_DelayedNotification);
            set.Bind(DelayedNotifBtn).To(vm => vm.DelayedNotificationCommand);

            set.Apply();
        }
    }
}