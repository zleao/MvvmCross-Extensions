using MvxExtensions.ViewModels;
using Playground.Forms.Core.ViewModels;
using Playground.Forms.UI.Core.Views;
using System;
using System.ComponentModel;
using Xamarin.Essentials;

namespace Playground.Forms.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class StoragePage : PlaygroundContentPage
    {
        public StoragePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if(status == PermissionStatus.Granted)
            {
                return;
            }

            status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                ((ViewModel)ViewModel).BackCommand.Execute(null);
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (ItemsCollectionView.SelectedItem != null)
            {
                ((StorageViewModel)ViewModel).CaseTestCommand.Execute(ItemsCollectionView.SelectedItem);
                ItemsCollectionView.SelectedItem = null;
            }
        }
    }
}
