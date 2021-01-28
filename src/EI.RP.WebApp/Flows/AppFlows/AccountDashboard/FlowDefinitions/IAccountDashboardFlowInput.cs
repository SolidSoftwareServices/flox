using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.AccountDashboard.FlowDefinitions
{
	public interface IAccountDashboardFlowInput : IFlowInputContract
	{
		string AccountNumber { get; set; }
		ResidentialPortalFlowType InitialFlow { get; set; }
		string InitialFlowStartType { get; set; }
		ResidentialPortalFlowType SourceFlowType { get; set; }
	}

	public class AccountDashboardFlowInput : IAccountDashboardFlowInput
	{
		public string AccountNumber { get; set; }
		public ResidentialPortalFlowType InitialFlow { get; set; }
		public string InitialFlowStartType { get; set; }
		public ResidentialPortalFlowType SourceFlowType { get; set; }
	}
}
