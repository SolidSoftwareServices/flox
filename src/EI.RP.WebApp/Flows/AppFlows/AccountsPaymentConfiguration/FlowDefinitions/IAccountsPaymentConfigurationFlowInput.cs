using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions
{
	public interface IAccountsPaymentConfigurationFlowInput : IFlowInputContract
	{
		AccountsPaymentConfigurationFlowStartType StartType { get; set; }
		string AccountNumber { get; set; }
		string SecondaryAccountNumber { get; set; }
	}

	public class AccountsPaymentConfigurationFlowInput : IAccountsPaymentConfigurationFlowInput
	{
		public AccountsPaymentConfigurationFlowStartType StartType { get; set; }
		public string AccountNumber { get; set; }
		public string SecondaryAccountNumber { get; set; }
		public bool IsDualFuelAccount { get; set; }
	}
}
