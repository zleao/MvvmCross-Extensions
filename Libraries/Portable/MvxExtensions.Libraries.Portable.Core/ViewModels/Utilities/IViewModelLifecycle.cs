namespace MvxExtensions.Libraries.Portable.Core.ViewModels.Utilities
{
    /// <summary>
    /// Interface for viewmodel lifecycle
    /// </summary>
    public interface IViewModelLifecycle
    {
        /// <summary>
        /// Gets a value indicating whether the correspondent view is visible or hidden.
        /// Controled by the method [IsVisible(bool value)]
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is view visible]; otherwise, <c>false</c>.
        /// </value>
        bool IsViewVisible { get; }

        /// <summary>
        /// Used to signal that a view is about to be shown/hidden
        /// </summary>
        /// <param name="value">if set to <c>true</c> the view is about to be shown. Othrewise is about to be hidden</param>
        void ChangeVisibility(bool value);

        /// <summary>
        /// Used to signal that the view is about to be destroyed
        /// </summary>
        void KillMe();
    }
}
