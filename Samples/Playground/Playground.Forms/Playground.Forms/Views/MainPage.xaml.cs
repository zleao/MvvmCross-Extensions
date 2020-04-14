using Playground.Forms.Core.ViewModels;
using Playground.Forms.UI.Core.Views;
using System;
using System.ComponentModel;

namespace Playground.Forms.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : PlaygroundContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
       
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (ItemsCollectionView.SelectedItem != null)
            {
                ((MainViewModel)ViewModel).NavigateCommand.Execute(ItemsCollectionView.SelectedItem);
                ItemsCollectionView.SelectedItem = null;
            }
        }
    }
}
