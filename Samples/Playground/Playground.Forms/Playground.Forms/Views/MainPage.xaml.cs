using Playground.Forms.Core.ViewModels;
using Playground.Forms.UI.Core.Views;
using System.ComponentModel;
using System.Linq;

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

        private void ItemsCollectionView_SelectionChanged(object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            ((MainViewModel)ViewModel).NavigateCommand.Execute(e.CurrentSelection.FirstOrDefault());
        }
    }
}
