

namespace EI.RP.WebApp.Flows.AppFlows.AccountAndMeterDetails.FlowDefinitions
{
	public abstract class AccountAndMeterDetailsScreen : ResidentialPortalScreen
	{
		public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.AccountAndMeterDetails;

		protected virtual string Title => "Details";
	}
}