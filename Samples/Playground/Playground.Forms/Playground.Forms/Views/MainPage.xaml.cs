using MvxExtensions.Forms.Views;
using Playground.Forms.Core.ViewModels;
using System.ComponentModel;

namespace Playground.Forms.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
       
        private void ItemsCollectionView_SelectionChanged(object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            if(ItemsCollectionView.SelectedItem != null)
            {
                ((MainViewModel)ViewModel).NavigateCommand.Execute(ItemsCollectionView.SelectedItem);
                ((Xamarin.Forms.CollectionView)sender).SelectedItem = null;
            }
        }
    }
}
