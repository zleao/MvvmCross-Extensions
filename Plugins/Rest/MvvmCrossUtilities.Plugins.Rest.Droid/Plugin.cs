using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace MvvmCrossUtilities.Plugins.Rest.Droid
{
    public class Plugin : IMvxPlugin
    {
        #region Implementation of IMvxPlugin

        public void Load()
        {
            Mvx.RegisterSingleton<IRestClient>(new RestClient());
        }

        #endregion
    }
}