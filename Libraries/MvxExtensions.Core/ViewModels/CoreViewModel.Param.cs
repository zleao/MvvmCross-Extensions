using MvvmCross.Logging;
using MvvmCross.ViewModels;

namespace MvxExtensions.Core.ViewModels
{
    /// <summary>
    /// Base viewmodel with input, built on top of <see cref="CoreViewModel"/>
    /// </summary>
    public abstract class CoreViewModel<TParameter> : CoreViewModel, IMvxViewModel<TParameter>
    {
        protected CoreViewModel(IMvxLogProvider logProvider)
            : base(logProvider)
        {
        }

        public abstract void Prepare(TParameter parameter);
    }
}