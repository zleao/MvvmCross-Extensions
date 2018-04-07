using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Libraries.Portable.Messages.TwoWay;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class DialogViewModel : AllAroundViewModel
    {
        #region Fields

        private Random _random = new Random();

        #endregion

        #region Properties

        public override string PageTitle
        {
            get { return "Dialog demos"; }
        }

        public string DialogTitle
        {
            get { return "TITLE" + Environment.NewLine + "SubTitle" + Environment.NewLine + "subsubtitle" + Environment.NewLine + "subsubsubtitle"; }
        }

        public IList<string> Items { get { return _items; } }
        private readonly IList<string> _items;

        #endregion

        #region Commands

        public ICommand ShowBlockingErrorCommand
        {
            get { return _showBlockingErrorCommand; }
        }
        private ICommand _showBlockingErrorCommand;

        public ICommand ShowGenericQuestionCommand
        {
            get { return _showGenericQuestionCommand; }
        }
        private ICommand _showGenericQuestionCommand;

        public ICommand ShowQuestionWithCustomAnswersCommand
        {
            get { return _showQuestionWithCustomAnswersCommand; }
        }
        private ICommand _showQuestionWithCustomAnswersCommand;

        #endregion

        #region Constructor

        public DialogViewModel()
        {
            _items = new List<string>() { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };

            _showBlockingErrorCommand = new MvxCommand(OnShowBlockingError);
            _showGenericQuestionCommand = new MvxCommand(OnShowGenericQuestion);
            _showQuestionWithCustomAnswersCommand = new MvxCommand(OnShowQuestionWithCustomAnswers);
        }

        #endregion

        #region Methods

        private async void OnShowBlockingError()
        {
            await PublishErrorNotificationAsync("Blocking error message demo", NotificationModeEnum.MessageBox);
        }

        private async void OnShowGenericQuestion()
        {
            var answer = await PublishGenericQuestionNotificationAsync("Do you feel lucky?", NotificationTwoWayAnswersGroupEnum.YesNo);
            await OnGenericQuestionResultAsync(answer);
        }
        private Task OnGenericQuestionResultAsync(NotificationGenericQuestionResult obj)
        {
            return PublishInfoNotificationAsync(obj.Answer.ToString());
        }

        private async void OnShowQuestionWithCustomAnswers()
        {
            var answer = await PublishQuestionWithCustomAnswerNotificationAsync("What's the number I'm thinking!", Items);
            await OnQuestionWithCustomAnswersResultAsync(answer);
        }
        private Task OnQuestionWithCustomAnswersResultAsync(NotificationQuestionCustomAnswerResult obj)
        {
            var number = _random.Next(0, 10);
            if (obj.SelectedAnswerIndex == number)
                return PublishInfoNotificationAsync("Correct!");
            else
                return PublishInfoNotificationAsync("Try again... The number was " + number);
        }

        #endregion
    }
}
