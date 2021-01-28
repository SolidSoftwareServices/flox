using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.Help.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.Help.Steps
{
    public class Help : HelpScreen
    {
	    public override ScreenName ScreenStep => HelpStep.Help;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var screenModel = new HelpScreenModel();

            SetTitle(Title, screenModel);

            return screenModel;
        }

        public class HelpScreenModel : UiFlowScreenModel
        {
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == HelpStep.Help;
            }
        }
    }
}