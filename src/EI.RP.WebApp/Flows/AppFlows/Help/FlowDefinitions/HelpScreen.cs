

namespace EI.RP.WebApp.Flows.AppFlows.Help.FlowDefinitions
{
	public abstract class HelpScreen : ResidentialPortalScreen
	{
		public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.Help;

		protected virtual string Title => "Help";
	}
}