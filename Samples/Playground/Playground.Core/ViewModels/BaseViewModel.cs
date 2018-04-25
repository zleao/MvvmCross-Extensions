using MvvmCross.Localization;
using MvvmCross.ViewModels;

namespace Playground.Core.ViewModels
{
    public abstract class BaseViewModel : MvxViewModel
    {
        #region Properties

        public IMvxLanguageBinder TextSourceCommon => new MvxLanguageBinder("Playground.Core", "Common");

        public IMvxLanguageBinder TextSource => new MvxLanguageBinder("Playground.Core", GetType().Name);

        #endregion
    }
}
