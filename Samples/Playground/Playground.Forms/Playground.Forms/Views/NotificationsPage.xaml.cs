using Playground.Forms.UI.Core.Views;
using System.ComponentModel;
using Xamarin.Forms.Xaml;

namespace Playground.Forms.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationsPage : PlaygroundContentPage
    {
        public NotificationsPage()
        {
            InitializeComponent();
        }
    }
}
