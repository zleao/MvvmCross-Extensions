using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;

namespace MvxExtensions.Libraries.Portable.Core.Views
{
    /// <summary>
    /// Implements <see cref="IMvxViewDispatcher"/>
    /// </summary>
    public interface IViewDispatcher : IMvxViewDispatcher
    {
        /// <summary>
        /// Signals the platform to show a new view, based on the mapped viewmodel.
        /// There's the option to either remove the current view from backstack oir to clear the back stack completely (including the current view)
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="removeCurrentViewFromStack">if set to <c>true</c> removes the current view from application stack.</param>
        /// <param name="clearBackStack">if set to <c>true</c> clears the entire back stack of the application.</param>
        /// <returns></returns>
        bool ShowViewModel(MvxViewModelRequest request, bool removeCurrentViewFromStack, bool clearBackStack);
    }
}
