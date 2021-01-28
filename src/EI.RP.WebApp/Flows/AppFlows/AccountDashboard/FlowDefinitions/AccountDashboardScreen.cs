namespace EI.RP.WebApp.Flows.AppFlows.AccountDashboard.FlowDefinitions
{
	public abstract class AccountDashboardScreen : ResidentialPortalScreen
	{
		public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.Accounts;
	}
}