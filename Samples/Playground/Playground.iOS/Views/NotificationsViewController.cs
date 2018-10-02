using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvxExtensions.Platforms.iOS.Views;
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
        }
    }
}