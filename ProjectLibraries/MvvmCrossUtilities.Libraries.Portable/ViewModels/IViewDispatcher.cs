using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Views;

namespace MvvmCrossUtilities.Libraries.Portable.ViewModels
{
    /// <summary>
    /// Interface for version of MvxViewDispatcher
    /// </summary>
    public interface IViewDispatcher : IMvxViewDispatcher
    {
        /// <summary>
        /// Shows a view model.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="removeCurrentViewFromStack">if set to <c>true</c> [remove current view from stack].</param>
        /// <returns></returns>
        bool ShowViewModel(MvxViewModelRequest request, bool removeCurrentViewFromStack);
    }
}
