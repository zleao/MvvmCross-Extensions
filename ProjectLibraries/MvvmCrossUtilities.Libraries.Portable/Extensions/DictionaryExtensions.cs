using System;
using System.Collections.Generic;
using MvvmCrossUtilities.Libraries.Portable.ViewModels;

namespace MvvmCrossUtilities.Libraries.Portable.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddOrUpdate(this IDictionary<string, Type> source, string key, Type value)
        {
            if (source == null)
                return;

            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, value);
        }

        public static void AddOrUpdate(this IDictionary<string, ViewModel> source, string key, ViewModel value)
        {
            if (source == null)
                return;

            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, value);
        }
    }
}
