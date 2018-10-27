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
    public partial class NavigationViewController : ViewControllerBase<NavigationViewModel>
    {
        public NavigationViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel?.PageTitle;

            var set = this.CreateBindingSet<NavigationViewController, NavigationViewModel>();

            set.Bind(LabelNavigateAndRemoveSelf).For(LabelNavigateAndRemoveSelf.Text).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(TextResourcesKeys.Text_Description_NavigateAndRemoveSelf);
            set.Bind(ButtonNavigateAndRemoveSelf).For(nameof(ButtonNavigateAndRemoveSelf.Title)).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(TextResourcesKeys.Label_Button_NavigateAndRemoveSelf);
            set.Bind(ButtonNavigateAndRemoveSelf).To(vm => vm.NavigateAndRemoveSelfCommand);

            set.Bind(LabelNavigate).For(LabelNavigateAndRemoveSelf.Text).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(TextResourcesKeys.Text_Description_Navigate);
            set.Bind(ButtonNavigate).For(nameof(ButtonNavigate.Title)).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(TextResourcesKeys.Label_Button_Navigate);
            set.Bind(ButtonNavigate).To(vm => vm.NavigateCommand);

            set.Apply();
        }
    }
}