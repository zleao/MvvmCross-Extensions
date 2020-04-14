using MvvmCross.Forms.Platforms.Uap.Core;
using MvvmCross.Forms.Platforms.Uap.Views;

namespace Playground.Forms.UWP
{
    public sealed partial class App : ProxyMvxApplication
    {
        public App()
        {
            this.InitializeComponent();
        }
    }

    public abstract class ProxyMvxApplication : MvxWindowsApplication<MvxFormsWindowsSetup<Core.App, Forms.App>, Core.App, Forms.App, MainPage>
    {
    }
}
