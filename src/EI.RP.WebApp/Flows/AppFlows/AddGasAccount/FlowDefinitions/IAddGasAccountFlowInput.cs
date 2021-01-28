using EI.RP.UiFlows.Core.Flows.Initialization;


namespace EI.RP.WebApp.Flows.AppFlows.AddGasAccount.FlowDefinitions
{
	public interface IAddGasAccountFlowInput : IFlowInputContract
	{
		string ElectricityAccountNumber { get; set; }
	}

	public class AddGasAccountFlowInput : IAddGasAccountFlowInput
	{
		public string ElectricityAccountNumber { get; set; }
	}
}