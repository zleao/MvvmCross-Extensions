using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvxExtensions.Platforms.iOS.Views;
using Playground.Core.ViewModels;
using System;
using System.Collections.Generic;
using UIKit;

namespace Playground.iOS
{
    [MvxFromStoryboard("Main")]
    [MvxRootPresentation(WrapInNavigationController = true)]
    public partial class MainViewController : TableViewControllerBase<MainViewModel>
    {
        private static readonly NSString Identifier = new NSString("MenuOptionCellIdentifier");
        private const string BindingText = "TitleText Text";

        public MainViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            Title =  ViewModel?.PageTitle;

            var source = new MvxStandardTableViewSource(TableView, UITableViewCellStyle.Default, Identifier, BindingText)
            {
                DeselectAutomatically = true
            };

            this.AddBindings(new Dictionary<object, string>
                {
                    {source, $"{nameof(source.ItemsSource)} {nameof(ViewModel.MenuOptions)};{nameof(source.SelectionChangedCommand)} {nameof(ViewModel.NavigateCommand)}"}
                });

            TableView.Source = source;
            TableView.ReloadData();
        }
    }
}