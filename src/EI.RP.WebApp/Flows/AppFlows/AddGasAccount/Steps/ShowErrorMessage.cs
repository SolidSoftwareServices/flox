using EI.RP.WebApp.Flows.AppFlows.AddGasAccount.FlowDefinitions;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.AddGasAccount.Steps
{
    public class ShowErrorMessage: AddGasAccountScreen
    {
	    public override ScreenName ScreenStep => AddGasAccountStep.ShowErrorMessage;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var screenModel = new ScreenModel();

            SetTitle(Title, screenModel);

            return screenModel;
        }


        public class ScreenModel : UiFlowScreenModel
        {
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == AddGasAccountStep.ShowErrorMessage;
            }
        }
    }
}
