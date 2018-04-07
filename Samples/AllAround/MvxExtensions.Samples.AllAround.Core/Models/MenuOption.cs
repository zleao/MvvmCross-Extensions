using System;

namespace MvxExtensions.Samples.AllAround.Core.Models
{
    public sealed class MenuOption
    {
        #region Properties

        public string Text
        {
            get { return _text; }
        }
        private readonly string _text;

        public Type ViewModelType
        {
            get { return _viewModelType; }
        }
        private readonly Type _viewModelType;

        #endregion

        #region Constructor

        public MenuOption(string text, Type viewModelType)
        {
            _text = text;
            _viewModelType = viewModelType;
        }

        #endregion
    }
}
