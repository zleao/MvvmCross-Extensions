using System;
using System.Collections;

namespace MvxExtensions.Libraries.Droid.Core.Support.V4.Components.Adapters
{
    public interface IPagerAdapter
    {
        string TitlePropertyName { get; set; }

        IEnumerable ItemsSource { get; set; }

        int Count { get; }

        int ItemTemplateId { get; set; }

        int GetPosition(object item);

        event EventHandler OnAfterDataSetChanged;
    }
}