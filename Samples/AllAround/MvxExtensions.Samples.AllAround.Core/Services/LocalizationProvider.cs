using System.Collections.Generic;
using MvvmCross.Plugins.JsonLocalization;

namespace MvxExtensions.Samples.AllAround.Core.Services
{
    public class LocalizationProvider : MvxTextProviderBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationProvider"/> class.
        /// </summary>
        /// <param name="localizationFolder">The localisation folder.</param>
        public LocalizationProvider(string localizationFolder)
            : base("MvxExtensions", "TextResources")
        {
            this.LoadResources(localizationFolder);
        }

        protected override IDictionary<string, string> ResourceFiles
        {
            get
            {
                if (_dictionary == null)
                {
                    _dictionary = new Dictionary<string, string>();
                    _dictionary.Add("TextResources", "TextResources");
                }

                return _dictionary;
            }
        }
        private IDictionary<string, string> _dictionary;
    }
}
