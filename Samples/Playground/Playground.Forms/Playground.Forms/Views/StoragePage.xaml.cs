using MvvmCross.Forms.Views;
using MvxExtensions.ViewModels;
using System.ComponentModel;
using Xamarin.Essentials;

namespace Playground.Forms.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class StoragePage : MvxContentPage
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
        private void ItemsCollectionView_SelectionChanged(object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            //((MainViewModel)ViewModel).NavigateCommand.Execute(e.CurrentSelection.FirstOrDefault());
        }
    }
}
