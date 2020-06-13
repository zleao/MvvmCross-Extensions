using MvvmCross.Logging;
using MvvmCross.ViewModels;
using System.Threading.Tasks;

namespace MvxExtensions.Core.ViewModels
{
    public abstract class CoreViewModelResult<TResult> : CoreViewModel, IMvxViewModelResult<TResult>
    {
        protected CoreViewModelResult(IMvxLogProvider logProvider) : base(logProvider)
        {
        }

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            if (viewFinishing && CloseCompletionSource?.Task.IsCompleted == false && !CloseCompletionSource.Task.IsFaulted)
                CloseCompletionSource?.TrySetCanceled();

            base.ViewDestroy(viewFinishing);
        }
    }
}
