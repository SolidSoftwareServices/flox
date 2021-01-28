using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.Agent.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.Agent.Steps
{
    public class AgentUiFlowContainerScreenStep : UiFlowContainerScreen<ResidentialPortalFlowType>
    {
	    protected static string Title => "Agent Dashboard";

        public static class StepEvent
        {
            public static readonly ScreenEvent ToChangePassword = new ScreenEvent(nameof(AgentUiFlowContainerScreenStep),nameof(ToChangePassword));
        }

        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.Agent;

        public override ScreenName ScreenStep => FlowDefinitions.AgentStep.AgentUiFlowContainerScreenStep;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
	        var result = new AgentUiFlowContainerScreenStepUiFlowScreenModel();

	        result.SetContainedFlow(GetInitialFlowStep(contextData));

	        await SetTitle(contextData, result);

	        return result;
        }

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var refreshedStepData = originalScreenModel.CloneDeep();

			await SetTitle(contextData, (AgentUiFlowContainerScreenStepUiFlowScreenModel)refreshedStepData);

			return refreshedStepData;
		}

		private ResidentialPortalFlowType GetInitialFlowStep(IUiFlowContextData contextData)
        {
            if (contextData.CurrentEvents.Any(x => x == StepEvent.ToChangePassword))
            {
                return ResidentialPortalFlowType.ChangePassword;
            }

            return ResidentialPortalFlowType.BusinessPartnersSearch;
        }

        private static async Task SetTitle(IUiFlowContextData contextData, AgentUiFlowContainerScreenStepUiFlowScreenModel model)
        {
	        var last = (await contextData.GetCurrentStepContainedData(ContainedFlowQueryOption.Last))
		        ?.ScreenTitle;
	        var immediate = (await contextData.GetCurrentStepContainedData(ContainedFlowQueryOption.Immediate))
		        ?.ScreenTitle;

	        model.ScreenTitle = last ?? immediate ?? Title;
        }

        public class AgentUiFlowContainerScreenStepUiFlowScreenModel : UiFlowScreenModel
        {
	        public AgentUiFlowContainerScreenStepUiFlowScreenModel() : base(true)
	        {
	        }

	        public override bool IsValidFor(ScreenName screenStep)
	        {
		        return screenStep == AgentStep.AgentUiFlowContainerScreenStep;
	        }
        }
    }
}