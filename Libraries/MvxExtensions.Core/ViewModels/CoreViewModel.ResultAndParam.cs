using MvvmCross.Logging;
using MvvmCross.ViewModels;

namespace MvxExtensions.Core.ViewModels
{
    public abstract class CoreViewModel<TParameter, TResult> : CoreViewModelResult<TResult>, IMvxViewModel<TParameter, TResult>
    {
        protected CoreViewModel(IMvxLogProvider logProvider) : base(logProvider)
        {
        }

        public abstract void Prepare(TParameter parameter);
    }
}
