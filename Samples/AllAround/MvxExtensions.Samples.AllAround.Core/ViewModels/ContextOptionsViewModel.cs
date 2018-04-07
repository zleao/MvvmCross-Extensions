using MvvmCrossUtilities.Libraries.Portable.Models;
using MvvmCrossUtilities.Samples.AllAround.Core.Resources;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class ContextOptionsViewModel : AllAroundViewModel
    {
        #region Constants

        private const string CONTEXTOPTION_VISIBLE_IF_POSSIBLE = "CONTEXTOPTION_VISIBLE_IF_POSSIBLE";
        private const string CONTEXTOPTION_NOT_VISIBLE = "CONTEXTOPTION_NOT_VISIBLE";

        private const string CONTEXTOPTION_IMAGEID_VISIBLE_IF_POSSIBLE = "visible";
        private const string CONTEXTOPTION_IMAGEID_NOT_VISIBLE = "not_visible";

        #endregion
        
        #region Properties

        public override string PageTitle
        {
            get { return "Context Options"; }
        }

        #endregion

        #region Methods
        
        protected override IDictionary<string, ContextOption> CreateContextOptions()
        {
            var options = base.CreateContextOptions();

            options.Add(CONTEXTOPTION_VISIBLE_IF_POSSIBLE, new ContextOption(CONTEXTOPTION_VISIBLE_IF_POSSIBLE,
                                                                             TextSource.GetText(TextResourcesKeys.Label_Button_ContextOption_Visible),
                                                                             true,
                                                                             !IsBusy,
                                                                             ProcessContextOptionCommand,
                                                                             CONTEXTOPTION_IMAGEID_VISIBLE_IF_POSSIBLE));

            options.Add(CONTEXTOPTION_NOT_VISIBLE, new ContextOption(CONTEXTOPTION_NOT_VISIBLE,
                                                                     TextSource.GetText(TextResourcesKeys.Label_Button_ContextOption_Not_Visible),
                                                                     true,
                                                                     !IsBusy,
                                                                     ProcessContextOptionCommand,
                                                                     CONTEXTOPTION_IMAGEID_NOT_VISIBLE,
                                                                     false));

            return options;
        }

        protected override async Task ProcessContextOptionAsync(string selectedOption)
        {
            if (selectedOption == CONTEXTOPTION_VISIBLE_IF_POSSIBLE)
                await PublishInfoNotificationAsync("Visible ContextOption selected");
            else if (selectedOption == CONTEXTOPTION_NOT_VISIBLE)
                await PublishInfoNotificationAsync("Not Visible ContextOption selected");
            else
                await base.ProcessContextOptionAsync(selectedOption);
        }

        #endregion
    }
}
