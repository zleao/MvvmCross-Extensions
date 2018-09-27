using MvvmCross.ViewModels;
using MvxExtensions.ViewModels;

namespace MvxExtensions
{
    public class Application : MvxApplication
    {
        protected override IMvxViewModelLocator CreateDefaultViewModelLocator()
        {
            return new SingletonViewModelLocator();
        } 
    }
}
