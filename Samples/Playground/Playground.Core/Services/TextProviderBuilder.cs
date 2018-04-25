using MvvmCross.IoC;
using MvvmCross.Plugin.JsonLocalization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Playground.Core.Services
{
    public class TextProviderBuilder : MvxTextProviderBuilder
    {
        public TextProviderBuilder(string localizationFolder)
            : base("Playground.Core", "Resources", new MvxEmbeddedJsonDictionaryTextProvider(false))
        {
            if (!string.IsNullOrWhiteSpace(localizationFolder))
            {
                LoadResources(localizationFolder);
            }
        }

        protected override IDictionary<string, string> ResourceFiles
        {
            get
            {
                var dictionary = GetType().GetTypeInfo()
                                     .Assembly
                                     .CreatableTypes()
                                     .Where(t => t.Name.EndsWith("ViewModel"))
                                     .ToDictionary(t => t.Name, t => t.Name);

                dictionary.Add("Common", "Common");

                return dictionary;
            }
        }
    }
}