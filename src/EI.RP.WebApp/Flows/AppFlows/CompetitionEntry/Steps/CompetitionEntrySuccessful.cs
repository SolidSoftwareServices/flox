using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.WebApp.Flows.AppFlows.CompetitionEntry.FlowDefinitions;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.CompetitionEntry.Steps
{
    public class CompetitionEntrySuccessful : CompetitionEntryScreen
    {
        public override ScreenName ScreenStep => CompetitionEntryStep.CompetitionEntrySuccessful;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            return contextData.GetStepData<CompetitionEntry.ScreenModel>();
        }
    }
}
