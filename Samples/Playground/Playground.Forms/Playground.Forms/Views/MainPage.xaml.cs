using MvvmCross.Forms.Views;
using Playground.Core.ViewModels;
using System.ComponentModel;
using System.Linq;

namespace Playground.Forms.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MvxContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ItemsCollectionView_SelectionChanged(object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            ((MainViewModel)ViewModel).NavigateCommand.Execute(e.CurrentSelection.FirstOrDefault());
        }
    }
}
