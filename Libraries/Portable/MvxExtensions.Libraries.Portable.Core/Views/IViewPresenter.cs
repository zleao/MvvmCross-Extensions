using MvvmCross.Core.ViewModels;

namespace MvxExtensions.Libraries.Portable.Core.Views
{
    /// <summary>
    /// Extension interface for the view presenter
    /// </summary>
    public interface IViewPresenter
    {
        /// <summary>
        /// Shows the specified viewmodel.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="removeCurrentViewFromStack">if set to <c>true</c> removes the current view from stack.</param>
        /// <param name="clearBackStack">if set to <c>true</c> clears the entire back stack.</param>
        void Show(MvxViewModelRequest request, bool removeCurrentViewFromStack, bool clearBackStack);
    }
}
