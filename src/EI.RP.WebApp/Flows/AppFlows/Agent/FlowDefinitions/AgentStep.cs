using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.Agent.FlowDefinitions
{
	public static class AgentStep
	{
        public static readonly ScreenName AgentUiFlowContainerScreenStep = new ScreenName(nameof(AgentUiFlowContainerScreenStep));

        public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
    }
}