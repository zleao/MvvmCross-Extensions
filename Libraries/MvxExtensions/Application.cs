using MvvmCross.ViewModels;
using MvxExtensions.ViewModels;

namespace MvxExtensions
{
    /// <inheritdoc/>
    public class Application : MvxApplication
    {
        protected override IMvxViewModelLocator CreateDefaultViewModelLocator()
        {
            return new SingletonViewModelLocator();
        }
    }
}
