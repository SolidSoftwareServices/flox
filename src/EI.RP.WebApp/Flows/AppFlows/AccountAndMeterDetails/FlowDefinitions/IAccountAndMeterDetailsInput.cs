using EI.RP.UiFlows.Core.Flows.Initialization;

namespace EI.RP.WebApp.Flows.AppFlows.AccountAndMeterDetails.FlowDefinitions
{
	public interface IAccountAndMeterDetailsInput : IFlowInputContract
	{
		string AccountNumber { get; set; }
	}

    public class AccountAndMeterDetailsInput : IAccountAndMeterDetailsInput
    {
        public string AccountNumber { get; set; }
    }
}
