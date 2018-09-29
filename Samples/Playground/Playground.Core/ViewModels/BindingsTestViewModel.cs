using MvvmCross.Base;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvxExtensions.Attributes;
using MvxExtensions.Plugins.Notification;
using Playground.Core.Resources;

namespace Playground.Core.ViewModels
{
    public class BindingsTestViewModel : BaseViewModel
    {
        #region Properties

        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set => SetProperty(ref _isValid, value);
        }

        private string _validityText;
        public string ValidityText
        {
            get => _validityText;
            set => SetProperty(ref _validityText, value);
        }

        private int _textMaxLength;
        public int TextMaxLength
        {
            get => _textMaxLength;
            set => SetProperty(ref _textMaxLength, value);
        }

        #endregion

        #region Constructor

        public BindingsTestViewModel(IMvxJsonConverter jsonConverter,
                                     INotificationService notificationManager,
                                     IMvxLogProvider logProvider,
                                     IMvxNavigationService navigationService)
            : base(nameof(BindingsTestViewModel), jsonConverter, notificationManager, logProvider, navigationService)
        {
            TextMaxLength = 5;
            OnIsValidChange();
        }

        #endregion

        #region Methods

        [DependsOn(nameof(IsValid))]
        private void OnIsValidChange()
        {
            ValidityText = IsValid
                ? TextSource.GetText(TextResourcesKeys.Text_Validity_Valid)
                : TextSource.GetText(TextResourcesKeys.Text_Validity_Invalid);
        }

        #endregion
    }
}
