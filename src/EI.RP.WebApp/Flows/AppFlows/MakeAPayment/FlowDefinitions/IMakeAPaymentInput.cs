using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Steps;

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.FlowDefinitions
{
	public interface IMakeAPaymentInput : IFlowInputContract
	{
		string AccountNumber { get; set; }
		PaymentFlowInitializer.StartType StartType { get; set; }
	}

    public class MakeAPaymentInput : IMakeAPaymentInput
    {
        public string AccountNumber { get; set; }
        public PaymentFlowInitializer.StartType StartType { get; set; }
    }
}