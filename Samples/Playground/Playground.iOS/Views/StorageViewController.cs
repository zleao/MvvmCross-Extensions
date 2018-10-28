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
    [MvxChildPresentation]
    public partial class StorageViewController : TableViewControllerBase<StorageViewModel>
    {
        static readonly NSString CellIdentifier = new NSString("NotStorageUseCaseCell");
        const string BindingText = "TitleText Name; DetailText Description";

        public StorageViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel?.PageTitle;

            var source = new MvxStandardTableViewSource(TableView, UITableViewCellStyle.Subtitle,CellIdentifier, BindingText)
            {
                DeselectAutomatically = true
            };

            this.AddBindings(new Dictionary<object, string>
                {
                    {source, $"{nameof(source.ItemsSource)} {nameof(ViewModel.CaseTests)};{nameof(source.SelectionChangedCommand)} {nameof(ViewModel.CaseTestCommand)}"}
                });

            TableView.Source = source;
            TableView.ReloadData();
        }
    }
}