using UIKit;

namespace MvxExtensions.Platforms.iOS.Components.Interfaces
{
    public interface ILoadingIndicator
    {
        void Show(UIView targetView, bool endEditing = true);
        void Hide();
    }
}
