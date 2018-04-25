using System;

namespace Playground.Core.Models
{
    public sealed class MenuOption
    {
        #region Properties

        public string Text { get; }

        public Type ViewModelType { get; }

        #endregion

        #region Constructor

        public MenuOption(string text, Type viewModelType)
        {
            Text = text;
            ViewModelType = viewModelType;
        }

        #endregion
    }
}
