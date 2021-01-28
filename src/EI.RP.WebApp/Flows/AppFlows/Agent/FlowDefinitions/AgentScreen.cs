namespace EI.RP.WebApp.Flows.AppFlows.Agent.FlowDefinitions
{
	public abstract class AgentScreen : ResidentialPortalScreen
	{
		public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.Agent;
	}
}