using System.Collections.Generic;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;

namespace MvvmCrossUtilities.Samples.AllAround.Core.Services
{
    public class LocalisationProvider : MvxTextProviderBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalisationProvider"/> class.
        /// </summary>
        /// <param name="localisationFolder">The localisation folder.</param>
        public LocalisationProvider(string localisationFolder)
            : base("MvvmCrossUtilities", "TextResources")
        {
            this.LoadResources(localisationFolder);
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
