using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvxExtensions.Platforms.iOS.Views;
using Playground.Core.Resources;
using Playground.Core.ViewModels;
using System;
using UIKit;

namespace Playground.iOS
{
    [MvxFromStoryboard("Main")]
    [MvxChildPresentation]
    public partial class NavigationSecondViewController : ViewControllerBase<NavigationSecondViewModel>
    {
        public NavigationSecondViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel?.PageTitle;

            var set = this.CreateBindingSet<NavigationSecondViewController, NavigationSecondViewModel>();

            set.Bind(LabelNavigationModeDescription).To(vm => vm.NavigationModeDescription);

            set.Bind(LabelNavigateAndClearBackstack).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(TextResourcesKeys.Text_Description_NavigateAndClearStack);
            set.Bind(ButtonNavigateAndClearBackstack).For(nameof(ButtonNavigateAndClearBackstack.Title)).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(TextResourcesKeys.Label_Button_NavigateAndClearStack);
            set.Bind(ButtonNavigateAndClearBackstack).To(vm => vm.NavigateAndClearStackCommand);

            set.Apply();
        }
    }
}