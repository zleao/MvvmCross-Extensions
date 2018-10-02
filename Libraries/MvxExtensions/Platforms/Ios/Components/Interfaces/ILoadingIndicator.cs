namespace MvxExtensions.Platforms.iOS.Components.Interfaces
{
    public interface ILoadingIndicator
    {
        void Show(bool endEditing = true);
        void Hide();
    }
}
